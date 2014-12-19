using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using SoundCloud.Common;

namespace AudioPlaybackManager
{
    public sealed class AudioTaskManager
    {
        #region Singleton
        private static AudioTaskManager _managerInstance;
        public static AudioTaskManager ManagerInstance
        {
            get
            {
                if (_managerInstance == null)
                    _managerInstance = new AudioTaskManager();
                return _managerInstance;
            }
        }
        #endregion Singleton


        #region Properties

        public BaseTrack CurrentTrack
        {
            get { return _currentTrack; }
        }

        #endregion Properties


        #region Variables
        private AutoResetEvent _taskInitialized;

        private MediaPlayer _mediaPlayer;
        private List<BaseTrack> _playlist;

        private BaseTrack _currentTrack;
        private int _currentIndex;
        private TimeSpan _startPosition = TimeSpan.FromSeconds(0);
        #endregion Variables

        public event TypedEventHandler<AudioTaskManager, BaseTrack> TrackChanged;

        private AudioTaskManager()
        {
            _currentIndex = -1;
            _taskInitialized = new AutoResetEvent(false);

            // Set the mediaplayer handlers
            _mediaPlayer = BackgroundMediaPlayer.Current;
            _mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            _mediaPlayer.CurrentStateChanged += mediaPlayer_CurrentStateChanged;
            _mediaPlayer.MediaFailed += mediaPlayer_MediaFailed;

            StartBackgroundAudioTask();
        }

        private void StartBackgroundAudioTask()
        {
            AddMediaPlayerEventHandlers();
            bool result = _taskInitialized.WaitOne(2000);
            //Send message to initiate playback
            if (result)
            {
                var message = new ValueSet();
                message.Add(Constants.StartPlayback, "0");
                BackgroundMediaPlayer.SendMessageToBackground(message);
            }
            else
            {
                throw new Exception("Background Audio Task didn't start in expected time");
            }
        }

        #region Playlist Methods
        public void SetPlaylist(IEnumerable<BaseTrack> playlist)
        {
            _playlist = new List<BaseTrack>(playlist);

            if (_currentIndex != -1)
                _currentIndex = _playlist.IndexOf(_currentTrack);
        }

        public void NextTrack()
        {
            _currentIndex++;
            if (_currentIndex < _playlist.Count)
            {
                _currentTrack = _playlist[_currentIndex];
            }
            else
                _currentTrack = null;

            PlayTrack();
        }

        public void PreviousTrack()
        {
            _currentIndex--;
            if (_currentIndex >= 0)
            {
                _currentTrack = _playlist[_currentIndex];
            }
            else
                _currentTrack = null;

            PlayTrack();
        }

        public void PlayTrack()
        {
            if (_currentTrack != null)
            {
                BackgroundMediaPlayer.Current.AutoPlay = false;
                BackgroundMediaPlayer.Current.SetUriSource(_currentTrack.PlaybackUri);
            }
        }

        public void PlayTrack(string trackId, TimeSpan startPosition)
        {
            BaseTrack track;
            try
            {
                track = _playlist.Single(t => t.Id == trackId);
            }
            catch (ArgumentNullException e)
            {
                return;
            }
            catch (InvalidOperationException e)
            {
                return;
            }

            // Set current track and index
            _currentTrack = track;
            _currentIndex = _playlist.IndexOf(_currentTrack);

            _startPosition = startPosition;
            _mediaPlayer.AutoPlay = false;
            _mediaPlayer.Volume = 0;
            _mediaPlayer.SetUriSource(track.PlaybackUri);
        }

        public void PlayAllTracks()
        {
            _currentTrack = _playlist[0];
            _currentIndex = 0;
            PlayTrack();
        }

        public BaseTrack GetTrack(string id)
        {
            try
            {
                _currentTrack = _playlist.Single(t => t.Id == id);
                _currentIndex = _playlist.IndexOf(_currentTrack);
                PlayTrack();
                return _currentTrack;
            }
            catch (ArgumentNullException e)
            {
                return null;
            }
            catch (InvalidOperationException e)
            {
                return null;
            }
        }
        #endregion Playlist Methods

        public void PauseTrack()
        {
            var value = new ValueSet();
            value.Add(Constants.PausePlayback, "");
            BackgroundMediaPlayer.SendMessageToBackground(value);
        }

        #region MediaPlayer Handlers

        /// <summary>
        /// Handler for state changed event of Media Player
        /// </summary>
        void mediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {

            if (sender.CurrentState == MediaPlayerState.Playing && _startPosition != TimeSpan.FromSeconds(0))
            {
                // if the start position is other than 0, then set it now
                sender.Position = _startPosition;
                sender.Volume = 1.0;
                _startPosition = TimeSpan.FromSeconds(0);
                sender.PlaybackMediaMarkers.Clear();
            }
        }

        /// <summary>
        /// Fired when MediaPlayer is ready to play the track
        /// </summary>
        void MediaPlayer_MediaOpened(MediaPlayer sender, object args)
        {
            // wait for media to be ready
            sender.Play();
            // invoke handler
            //TrackChanged.Invoke(this, CurrentTrack);
        }

        /// <summary>
        /// Handler for MediaPlayer Media Ended
        /// </summary>
        private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
           // play next track
            NextTrack();
        }

        private void mediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
        }

        #endregion MediaPlayer Handlers

        #region Background Media Handlers

        /// <summary>
        /// Unsubscribes to MediaPlayer events. Should run only on suspend
        /// </summary>
        private void RemoveMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.Current.CurrentStateChanged -= this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground -= this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        /// <summary>
        /// Subscribes to MediaPlayer events
        /// </summary>
        private void AddMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.Current.CurrentStateChanged += this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground += this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        private void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            foreach (string key in e.Data.Keys)
            {
                switch (key)
                {
                    case Constants.Trackchanged:
                        break;
                    case Constants.BackgroundTaskStarted:
                        //Wait for Background Task to be initialized before starting playback
                        _taskInitialized.Set();
                        break;
                }
            }
        }

        private void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            throw new NotImplementedException();
        }

        #endregion Background Media Handlers
    }
}
