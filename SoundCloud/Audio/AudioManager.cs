using System;
using System.Collections.ObjectModel;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using BackgroundAudio.PlayQueue;
using BackgroundAudioTask;
using SoundCloud.Common;
using SoundCloud.Model;
using SoundCloud.Services.Utils;

namespace SoundCloud.Audio
{
    public class AudioManager
    {
        #region Events
        public delegate void StateChangedHandler(bool isPlaying);
        public delegate void TrackLoadedHandler(string id);

        public event EventHandler TrackChanged;
        public event StateChangedHandler PlayerStateChanged;
        public event TrackLoadedHandler TrackLoaded;

        #endregion Events

        #region Properties

        public bool IsPlaying
        {
            get { return _isPlaying; }
        }

        public bool HasTrackOpen
        {
            get { return _playQueueDb.GetCurrentTrack() != null; }
        }

        public string UserAuth { get; set; }

        #endregion Properties

        #region Variables
        private readonly SyncPlayQueue _playQueueDb;
        private readonly PlaylistManager _playlistManager;

        private bool _isPlaying;
        #endregion Variables

        public AudioManager()
        {
            _playlistManager = PlaylistManager.ManagerInstance;
            _playQueueDb = SyncPlayQueue.PlayQueueInstance;
            _isPlaying = false;
            AddMediaPlayerEventHandlers();
        }

        public void DisconnectFromPlayer()
        {
            RemoveMediaPlayerEventHandlers();
        }

        public BaseTrack GetCurrentTrack()
        {
            return _playQueueDb.GetCurrentTrack();
        }

        public bool EmptyPlaylist()
        {
            return _playQueueDb.EmptyQueue();
        }

        public void SetPlaylist(ObservableCollection<CollectionItem> playlist)
        {
            foreach (var collectionItem in playlist)
            {
                AddToPlaylist(collectionItem);
            }
        }

        public bool AddToPlaylist(CollectionItem collectionItem)
        {
            if (collectionItem.Type == "playlist" || collectionItem.Type == "playlist-repost")
            {
                // Soundcloud changed API for playlist in stream on 28-1, no time to update app.

                //bool success = false;
                //foreach (var playlistTrack in collectionItem.ItemPlaylist.Tracks)
                //{
                //    var track = new BaseTrack()
                //    {
                //        Artist = playlistTrack.User.UserName,
                //        Id = playlistTrack.Id + collectionItem.Id,
                //        ArtworkUri = playlistTrack.Artwork,
                //        PlaybackUri = new Uri(playlistTrack.StreamUrl).UriWithAuthorizedUri(UserAuth),
                //        Title = playlistTrack.Title,
                //        Duration = playlistTrack.Duration
                //    };
                //    success = _playQueueDb.AddOneToQueue(track);
                //}
                return true;
            }
            else
            {
                var track = new BaseTrack()
                {
                    Artist = collectionItem.ItemTrack.User.UserName,
                    Id = collectionItem.Id,
                    ArtworkUri = collectionItem.ItemTrack.Artwork,
                    PlaybackUri = new Uri(collectionItem.ItemTrack.StreamUrl).UriWithAuthorizedUri(UserAuth),
                    Title = collectionItem.ItemTrack.Title,
                    Duration = collectionItem.ItemTrack.Duration
                };
                return _playQueueDb.AddOneToQueue(track);
            }
        }

        public bool AddToPlaylist(Playlist playlist)
        {
            bool success = false;
            foreach (var track in playlist.Tracks)
            {
                var baseTrack = new BaseTrack()
                {
                    Artist = track.User.UserName,
                    Id = track.Id.ToString() + playlist.Id.ToString(),
                    ArtworkUri = track.Artwork,
                    PlaybackUri = new Uri(track.StreamUrl).UriWithAuthorizedUri(UserAuth),
                    Title = track.Title,
                    Duration = track.Duration
                };
                success = _playQueueDb.AddOneToQueue(baseTrack);
            }
            return success;
        }

        public bool AddToPlaylist(Track track)
        {
            var baseTrack = new BaseTrack()
            {
                Artist = track.User.UserName,
                Id = track.Id.ToString(),
                ArtworkUri = track.Artwork,
                PlaybackUri = new Uri(track.StreamUrl).UriWithAuthorizedUri(UserAuth),
                Title = track.Title,
                Duration = track.Duration
            };
            return _playQueueDb.AddOneToQueue(baseTrack);
        }

        #region Forground Media Handlers

        public TimeSpan CurrentTrackPosition()
        {
            return BackgroundMediaPlayer.Current.Position;
        }

        public void SeekInTrack(double seconds)
        {
            var message = new ValueSet();
            message.Add(Constants.SeekInTrack, seconds);
            BackgroundMediaPlayer.SendMessageToBackground(message);
        }

        public void PlayCurrentTrack()
        {
            // Send message to background for playback current song
            var message = new ValueSet();
            message.Add(Constants.StartPlayback, "");
            BackgroundMediaPlayer.SendMessageToBackground(message);
        }

        public void PlayTrack(string id)
        {
            // Send message to background for playback with song id
            var message = new ValueSet();
            message.Add(Constants.StartPlaybackWithId, "");
            message.Add("trackid", id);
            BackgroundMediaPlayer.SendMessageToBackground(message);
        }

        public void PauseTrack()
        {
            // Send message to background for pausing song
            var message = new ValueSet();
            message.Add(Constants.PausePlayback, "");
            BackgroundMediaPlayer.SendMessageToBackground(message);
        }

        public void NextTrack()
        {
            // Send message to background for next song
            var message = new ValueSet();
            message.Add(Constants.SkipNext, "");
            BackgroundMediaPlayer.SendMessageToBackground(message);
        }

        public void PreviousTrack()
        {
            // Send message to background for previous song
            var message = new ValueSet();
            message.Add(Constants.SkipPrevious, "");
            BackgroundMediaPlayer.SendMessageToBackground(message);
        }

        protected void OnTrackChanged()
        {
            if (TrackChanged != null)
                TrackChanged(this, new EventArgs());
        }

        protected void OnStateChanged(bool isPlaying)
        {
            if (PlayerStateChanged != null)
                PlayerStateChanged(isPlaying);

            if (TrackLoaded != null)
                TrackLoaded(_playQueueDb.GetCurrentTrack().Id);
        }

        #endregion Foreground Media Handlers

        #region Background Media Handlers

        /// <summary>
        /// Unsubscribes to MediaPlayer events. Should run only on suspend
        /// </summary>
        private void RemoveMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.Current.CurrentStateChanged -= this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground -=
                this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        /// <summary>
        /// Subscribes to MediaPlayer events
        /// </summary>
        private void AddMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.Current.CurrentStateChanged += this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground +=
                this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        private void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender,
            MediaPlayerDataReceivedEventArgs e)
        {
            foreach (string key in e.Data.Keys)
            {
                switch (key)
                {
                    case Constants.Trackchanged:
                        OnTrackChanged();
                        break;
                    case Constants.PausePlayback:
                        OnStateChanged(false);
                        break;
                    case Constants.StartPlayback:
                        OnStateChanged(true);
                        break;
                }
            }
        }

        private void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
        }

        #endregion Background Media Handlers
    }
}
