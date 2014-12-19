using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
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
            get { return _currentTrack; }
        }

        #endregion Properties

        #region Variables
        private BaseTrack _currentTrack;
        private int _currentIndex;
        private TimeSpan _startPosition = TimeSpan.FromSeconds(0);

        private MediaPlayer _mediaPlayer;
        private PlayQueueDB _playQueue;

        public event TypedEventHandler<PlaylistManager, BaseTrack> TrackChanged;

        #endregion Variables

        private PlaylistManager()
        {
            _playQueue = new PlayQueueDB();

            // Set the mediaplayer handlers
            _mediaPlayer = BackgroundMediaPlayer.Current;
            _mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            _mediaPlayer.CurrentStateChanged += mediaPlayer_CurrentStateChanged;
            _mediaPlayer.MediaFailed += mediaPlayer_MediaFailed;
        }

        #region Playlist Methods

        public void SetPlaylist(IEnumerable<BaseTrack> playlist)
        {
            _playQueue.SetQueue(playlist);
        }

        public void AddToPlaylist(IEnumerable<BaseTrack> playlist)
        {
            _playQueue.AddToQueue(playlist);
        }

        public void NextTrack()
        {
            _currentTrack = _playQueue.GetNext(_currentTrack.dbId);
            PlayTrack();
        }

        public void PreviousTrack()
        {
            _currentTrack = _playQueue.GetPrevious(_currentTrack.dbId);
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
                track = _playQueue.GetTrack(trackId);
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

            _startPosition = startPosition;
            _mediaPlayer.AutoPlay = false;
            _mediaPlayer.Volume = 0;
            _mediaPlayer.SetUriSource(track.PlaybackUri);
        }

        public void PlayAllTracks()
        {
            _currentTrack = _playQueue.GetFirst();
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
                sender.Volume = 1.0;
                _startPosition = TimeSpan.FromSeconds(0);
                sender.PlaybackMediaMarkers.Clear();
            }
        }

        /// <summary>
        /// Fired when MediaPlayer is ready to play the track
        /// </summary>
        private void MediaPlayer_MediaOpened(MediaPlayer sender, object args)
        {
            // wait for media to be ready
            sender.Play();
            // invoke handler
            //TrackChanged.Invoke(this, new BaseTrack());
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
    }
}
