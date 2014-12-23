using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        #region Singleton

        private static AudioManager _instance;

        public static AudioManager ManagerInstance
        {
            get
            {
                if (_instance == null)
                    _instance = new AudioManager();
                return _instance;
            }
        }

        #endregion Singleton

        #region Properties

        public bool IsPlaying
        {
            get { return _isPlaying; }
        }

        public string UserAuth { get; set; }

        #endregion Properties

        #region Variables
        private readonly SyncPlayQueue _playQueueDb;
        private readonly PlaylistManager _playlistManager;

        private bool _isPlaying;
        #endregion Variables

        private AudioManager()
        {
            _playlistManager = PlaylistManager.ManagerInstance;
            _playQueueDb = SyncPlayQueue.PlayQueueInstance;
            _isPlaying = false;
        }

        /* ASYNC
        public async Task<bool> EmptyPlaylist()
        {
            return await _playQueueDb.EmptyQueue();
        }

        public async void SetPlaylist(ObservableCollection<CollectionItem> playlist)
        {
            foreach (var collectionItem in playlist)
            {
                await AddToPlaylist(collectionItem);
            }
        }

        public async Task<bool> AddToPlaylist(CollectionItem collectionItem)
        {
            var track = new BaseTrack();
            if (collectionItem.Type == "playlist" || collectionItem.Type == "playlist-repost")
            {
                // TODO: For each track in playlist place in list
            }
            else
            {
                track = new BaseTrack()
                {
                    Artist = collectionItem.ItemTrack.User.UserName,
                    Id = collectionItem.Id,
                    PlaybackUri = new Uri(collectionItem.ItemTrack.StreamUrl).UriWithAuthorizedUri(UserAuth),
                    Title = collectionItem.ItemTrack.Title
                };
            }

            if (!string.IsNullOrEmpty(track.Id))
            {
                return await _playQueueDb.AddOneToQueue(track);
            }
            return false;
        }
        */

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
            var track = new BaseTrack();
            if (collectionItem.Type == "playlist" || collectionItem.Type == "playlist-repost")
            {
                // TODO: For each track in playlist place in list
            }
            else
            {
                track = new BaseTrack()
                {
                    Artist = collectionItem.ItemTrack.User.UserName,
                    Id = collectionItem.Id,
                    ArtworkUri = collectionItem.ItemTrack.Artwork,
                    PlaybackUri = new Uri(collectionItem.ItemTrack.StreamUrl).UriWithAuthorizedUri(UserAuth),
                    Title = collectionItem.ItemTrack.Title
                };
            }

            if (!string.IsNullOrEmpty(track.Id))
            {
                return _playQueueDb.AddOneToQueue(track);
            }
            return false;
        }


        public void PlayTrack(string id)
        {
            _playlistManager.PlayTrack(id, TimeSpan.FromSeconds(0));
        }

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
                        break;
                    case Constants.BackgroundTaskStarted:
                        //Wait for Background Task to be initialized before starting playback
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
