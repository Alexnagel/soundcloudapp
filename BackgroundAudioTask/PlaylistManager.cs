using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using BackgroundAudio.PlayQueue;
using SoundCloud.Common;

namespace BackgroundAudioTask
{
    public sealed class PlaylistManager
    {
        #region Singleton 

        private static PlaylistManager _managerInstance;

        public static PlaylistManager ManagerInstance
        {
            get
            {
                if (_managerInstance == null)
                    _managerInstance = new PlaylistManager();
                return _managerInstance;
            }
        }
        #endregion Singleton

        #region Properties
        public BaseTrack CurrentTrack
        {
            get { return _playQueue.GetCurrentTrack(); }
        }

        #endregion Properties

        #region Variables

        private BaseTrack _trackToPlay;
        private int _currentIndex;
        private TimeSpan _startPosition = TimeSpan.FromSeconds(0);

        private readonly MediaPlayer _mediaPlayer;
        private readonly SyncPlayQueue _playQueue;
        private QueueType _queueType;

        #endregion Variables

        private PlaylistManager()
        {
            _playQueue = SyncPlayQueue.PlayQueueInstance;

            // Set the mediaplayer handlers
            _mediaPlayer = BackgroundMediaPlayer.Current;
            _mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            _mediaPlayer.CurrentStateChanged += mediaPlayer_CurrentStateChanged;
            _mediaPlayer.MediaFailed += mediaPlayer_MediaFailed;
        }

        #region Playlist Methods

        public void SetQueueType(QueueType type)
        {
            _queueType = type;
        }

        public void NextTrack()
        {
            _trackToPlay = _playQueue.GetNext(CurrentTrack.dbId, _queueType);
            PlayTrack();
        }

        public void PreviousTrack()
        {
            _trackToPlay = _playQueue.GetPrevious(CurrentTrack.dbId, _queueType);
            PlayTrack();
        }

        public void PlayTrack()
        {
            if (_trackToPlay != null)
            {
                if (CurrentTrack != null && CurrentTrack.IsPlaying)
                    _playQueue.SetTrackPlaying(CurrentTrack.dbId, false);

                _playQueue.SetTrackPlaying(_trackToPlay.dbId, true);
                _queueType = _trackToPlay.Type;

                BackgroundMediaPlayer.Current.Volume = 0;
                BackgroundMediaPlayer.Current.AutoPlay = false;
                BackgroundMediaPlayer.Current.SetUriSource(_trackToPlay.PlaybackUri);
            }
        }

        public void PlayTrack(string trackId, TimeSpan startPosition, QueueType type)
        {
            BaseTrack track;
            try
            {
                track = _playQueue.GetTrack(trackId, type);
            }
            catch (ArgumentNullException e)
            {
                return;
            }
            catch (InvalidOperationException e)
            {
                return;
            }

            // Set current track
            _trackToPlay = track;

            _startPosition = startPosition;
            PlayTrack();
        }

        public void PlayAllTracks()
        {
            _trackToPlay = _playQueue.GetFirst();
            _currentIndex = 0;
            PlayTrack();
        }

        public void PlayAllTracksM()
        {
            var value = new ValueSet();
            value.Add(Constants.StartPlayback, "");
            BackgroundMediaPlayer.SendMessageToBackground(value);
        }

        /*
        public async Task<BaseTrack> GetTrack(string id)
        {
            try
            {
                _currentTrack = await _playQueue.GetTrack(id);
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
        }*/

        public void PauseTrack()
        {
            var value = new ValueSet();
            value.Add(Constants.PausePlayback, "");
            BackgroundMediaPlayer.SendMessageToBackground(value);
        }

        public BaseTrack GetCurrentTrack()
        {
            return _playQueue.GetCurrentTrack();
        }

        #endregion Playlist Methods
        
        #region MediaPlayer Handlers

        /// <summary>
        /// Handler for state changed event of Media Player
        /// </summary>
        private void mediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            if (sender.CurrentState == MediaPlayerState.Playing && _startPosition != TimeSpan.FromSeconds(0))
            {
                // if the start position is other than 0, then set it now
                sender.Position = _startPosition;
                _startPosition = TimeSpan.FromSeconds(0);
                sender.PlaybackMediaMarkers.Clear();
            }
            sender.Volume = 1.0;
        }

        /// <summary>
        /// Fired when MediaPlayer is ready to play the track
        /// </summary>
        private void MediaPlayer_MediaOpened(MediaPlayer sender, object args)
        {
            // wait for media to be ready
            if (sender.CurrentState != MediaPlayerState.Playing)
                sender.Play();
        }

        /// <summary>
        /// Handler for MediaPlayer Media Ended
        /// </summary>
        private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            // play next track
            _startPosition = TimeSpan.FromSeconds(0);
            NextTrack();
        }

        private void mediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
        }

        #endregion MediaPlayer Handlers
    }
}
