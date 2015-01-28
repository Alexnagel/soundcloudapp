using System;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using BackgroundAudio.PlayQueue;
using SoundCloud.Common;

namespace BackgroundAudioTask
{

    /// <summary>
    /// Enum to identify foreground app state
    /// </summary>
    enum ForegroundAppStatus
    {
        Active,
        Suspended,
        Unknown
    }

    public sealed class AudioTask : IBackgroundTask
    {
        private SystemMediaTransportControls _systemMediaTransportControls;
        private BackgroundTaskDeferral deferral; // Used to keep task alive
        private ForegroundAppStatus _foregroundAppStatus = ForegroundAppStatus.Unknown;
        private AutoResetEvent _backgroundTaskStarted = new AutoResetEvent(false);
        private bool _backgroundTaskRunning = false;

        private PlaylistManager _audioManager;

        private PlaylistManager audioManager
        {
            get
            {
                if (_audioManager == null)
                    _audioManager = PlaylistManager.ManagerInstance;
                return _audioManager;
            }
        }

        private BaseTrack _currentTrack;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
            _systemMediaTransportControls.ButtonPressed += systemMediaTransportControl_ButtonPressed;
            _systemMediaTransportControls.PropertyChanged += systemMediaTransportControl_PropertyChanged;
            _systemMediaTransportControls.IsEnabled = true;
            _systemMediaTransportControls.IsPauseEnabled = true;
            _systemMediaTransportControls.IsPlayEnabled = true;
            _systemMediaTransportControls.IsNextEnabled = true;
            _systemMediaTransportControls.IsPreviousEnabled = true;

            // Associate a cancellation and completed handlers with the background task.
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);
            taskInstance.Task.Completed += Taskcompleted;
            var value = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.AppState);
            if (value == null)
                _foregroundAppStatus = ForegroundAppStatus.Unknown;
            else
                _foregroundAppStatus = (ForegroundAppStatus)Enum.Parse(typeof(ForegroundAppStatus), value.ToString());

            //Add handlers for MediaPlayer
            BackgroundMediaPlayer.Current.CurrentStateChanged += Current_CurrentStateChanged;

            //Initialize message channel 
            BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayer_MessageReceivedFromForeground;

            //Send information to foreground that background task has been started if app is active
            if (_foregroundAppStatus != ForegroundAppStatus.Suspended)
            {
                ValueSet message = new ValueSet();
                message.Add(Constants.BackgroundTaskStarted, "");
                BackgroundMediaPlayer.SendMessageToForeground(message);
            }

            _backgroundTaskStarted.Set();
            _backgroundTaskRunning = true;

            deferral = taskInstance.GetDeferral();
        }

        /// <summary>
        /// Indicate that the background task is completed.
        /// </summary>       
        void Taskcompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            deferral.Complete();
        }

        /// <summary>
        /// Handles background task cancellation. Task cancellation happens due to :
        /// 1. Another Media app comes into foreground and starts playing music 
        /// 2. Resource pressure. Your task is consuming more CPU and memory than allowed.
        /// In either case, save state so that if foreground app resumes it can know where to start.
        /// </summary>
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // You get some time here to save your state before process and resources are reclaimed
            try
            {
                //save state
                ApplicationSettingsHelper.SaveSettingsValue(Constants.CurrentTrack, audioManager.CurrentTrack.Id);
                ApplicationSettingsHelper.SaveSettingsValue(Constants.Position, BackgroundMediaPlayer.Current.Position.ToString());
                ApplicationSettingsHelper.SaveSettingsValue(Constants.BackgroundTaskState, Constants.BackgroundTaskCancelled);
                ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Enum.GetName(typeof(ForegroundAppStatus), _foregroundAppStatus));

                _backgroundTaskRunning = false;
                //unsubscribe event handlers
                _systemMediaTransportControls.ButtonPressed -= systemMediaTransportControl_ButtonPressed;
                _systemMediaTransportControls.PropertyChanged -= systemMediaTransportControl_PropertyChanged;

                //clear objects task cancellation can happen uninterrupted
                BackgroundMediaPlayer.Shutdown(); // shutdown media pipeline
            }
            catch (Exception ex)
            {
            }
            deferral.Complete(); // signals task completion. 
        }

        #region SysteMediaTransportControls related functions and handlers
        /// <summary>
        /// Update UVC using SystemMediaTransPortControl apis
        /// </summary>
        private void UpdateUVCOnNewTrack()
        {
            var audioManagerCurrentTrack = audioManager.GetCurrentTrack();
            if (_currentTrack != audioManagerCurrentTrack)
            {
                _currentTrack = audioManagerCurrentTrack;
                _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                _systemMediaTransportControls.DisplayUpdater.Type = MediaPlaybackType.Music;
                _systemMediaTransportControls.DisplayUpdater.MusicProperties.Title = audioManagerCurrentTrack.Title;
                _systemMediaTransportControls.DisplayUpdater.MusicProperties.Artist = audioManagerCurrentTrack.Artist;
                _systemMediaTransportControls.DisplayUpdater.Update();
            }
        }

        /// <summary>
        /// Fires when any SystemMediaTransportControl property is changed by system or user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void systemMediaTransportControl_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {
            //TODO: If soundlevel turns to muted, app can choose to pause the music
        }

        /// <summary>
        /// This function controls the button events from UVC.
        /// This code if not run in background process, will not be able to handle button pressed events when app is suspended.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void systemMediaTransportControl_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    // If music is in paused state, for a period of more than 5 minutes, 
                    //app will get task cancellation and it cannot run code. 
                    //However, user can still play music by pressing play via UVC unless a new app comes in clears UVC.
                    //When this happens, the task gets re-initialized and that is asynchronous and hence the wait
                    if (!_backgroundTaskRunning)
                    {
                        bool result = _backgroundTaskStarted.WaitOne(2000);
                        if (!result)
                            throw new Exception("Background Task didnt initialize in time");
                    }
                    PlayTrack();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    PauseTrack();
                    break;
                case SystemMediaTransportControlsButton.Next:
                    NextTrack();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    PreviousTrack();
                    break;
            }
        }



        #endregion

        #region AudioManager

        public void PlayTrack()
        {
            try
            {
                if (audioManager.CurrentTrack == null)
                {
                    //If the task was cancelled we would have saved the current track and its position. We will try playback from there
                    var currentTrackId = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.CurrentTrack);
                    var currenttrackposition = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.Position);

                    if (currentTrackId != null)
                    {

                        if (currenttrackposition == null)
                        {
                            // play from start if we dont have position
                            audioManager.PlayTrack((string)currentTrackId, TimeSpan.FromSeconds(0));
                        }
                        else
                        {
                            // play from exact position otherwise
                            audioManager.PlayTrack((string)currentTrackId, TimeSpan.Parse((string)currenttrackposition));
                        }
                    }
                    else
                    {
                        //If we dont have anything, play from beginning of playlist.
                        audioManager.PlayAllTracks(); //start playback
                    }
                }
                else
                {
                    BackgroundMediaPlayer.Current.Play();
                }

                // Send message of play to foreground 
                ValueSet message = new ValueSet();
                message.Add(Constants.StartPlayback, "");
                BackgroundMediaPlayer.SendMessageToForeground(message);
            }
            catch (Exception ex)
            {
            }
        }

        public void PlayTrackById(string id)
        {
            audioManager.PlayTrack(id, TimeSpan.FromSeconds(0));

            // Send message of play to foreground 
            var message = new ValueSet {{Constants.StartPlayback, ""}};
            BackgroundMediaPlayer.SendMessageToForeground(message);
        }

        public void PauseTrack()
        {
            try
            {
                BackgroundMediaPlayer.Current.Pause();

                // Send message of pause to foreground 
                ValueSet message = new ValueSet();
                message.Add(Constants.PausePlayback, "");
                BackgroundMediaPlayer.SendMessageToForeground(message);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Play the next track in the playlist
        /// </summary>
        public void NextTrack()
        {
            _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;
            audioManager.NextTrack();
            
            // Send message of change 
            ValueSet message = new ValueSet();
            message.Add(Constants.Trackchanged, "");
            BackgroundMediaPlayer.SendMessageToForeground(message);
        }

        /// <summary>
        /// Play the previous track in the playlist
        /// </summary>
        public void PreviousTrack()
        {
            _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;
            audioManager.PreviousTrack();

            // Send message of change 
            ValueSet message = new ValueSet();
            message.Add(Constants.Trackchanged, "");
            BackgroundMediaPlayer.SendMessageToForeground(message);
        }

        public void SeekInTrack(double seconds)
        {
            BackgroundMediaPlayer.Current.Position = TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Fires when playlist changes to a new track
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void playList_TrackChanged(PlaylistManager sender, BaseTrack args)
        {
            UpdateUVCOnNewTrack();
            //ApplicationSettingsHelper.SaveSettingsValue(Constants.CurrentTrack, args.Id);

            if (_foregroundAppStatus == ForegroundAppStatus.Active)
            {
                //Message channel that can be used to send messages to foreground
                //ValueSet message = new ValueSet();
                //message.Add(Constants.Trackchanged, args.Title);
                //BackgroundMediaPlayer.SendMessageToForeground(message);
            }
        }

        #endregion AudioManager

        #region Background Media Player Handlers
        void Current_CurrentStateChanged(MediaPlayer sender, object args)
        {
            if (sender.CurrentState == MediaPlayerState.Playing)
            {
                _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                UpdateUVCOnNewTrack();
            }
            else if (sender.CurrentState == MediaPlayerState.Paused)
            {
                _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
            }
        }


        /// <summary>
        /// Fires when a message is recieved from the foreground app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            foreach (string key in e.Data.Keys)
            {
                switch (key.ToLower())
                {
                    case Constants.StartPlaybackWithId:
                        PlayTrackById((string)e.Data["trackid"]);
                        break;
                    case Constants.StartPlayback:
                        PlayTrack();
                        break;
                    case Constants.PausePlayback:
                        PauseTrack();
                        break;
                    case Constants.SkipNext:
                        NextTrack();
                        break;
                    case Constants.SkipPrevious:
                        PreviousTrack();
                        break;
                    case Constants.SeekInTrack:
                        SeekInTrack((double)e.Data[Constants.SeekInTrack]);
                        break;
                }
            }
        }
        #endregion
    }
}
