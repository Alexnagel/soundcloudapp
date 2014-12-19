using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
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

        public List<BaseTrack> Playlist
        {
            get { return _playlist; }
        }

        public string UserAuth { get; set; }

        #endregion Properties

        #region Variables

        private AutoResetEvent _taskInitialized;
        private List<BaseTrack> _playlist;

        private PlaylistManager _playlistManager;
        #endregion Variables

        private AudioManager()
        {
            _playlistManager = PlaylistManager.ManagerInstance;
            _taskInitialized = new AutoResetEvent(false);

            StartBackgroundAudioTask();
        }

        public async void SetPlaylist(ObservableCollection<CollectionItem> playlist)
        {
            _playlist = new List<BaseTrack>();

            foreach (var collectionItem in playlist)
            {
                if (collectionItem.Type == "playlist" || collectionItem.Type == "playlist-repost")
                {
                    // TODO: For each track in playlist place in list
                }
                else
                {
                    BaseTrack track = new BaseTrack()
                    {
                        Artist = collectionItem.ItemTrack.User.UserName,
                        Id = collectionItem.Id,
                        PlaybackUri = new Uri(collectionItem.ItemTrack.StreamUrl).UriWithAuthorizedUri(UserAuth),
                        Title = collectionItem.ItemTrack.Title
                    };
                    _playlist.Add(track);
                }
            }
            //_playlistManager.AddToPlaylist(_playlist);
        }

        private void StartBackgroundAudioTask()
        {
            AddMediaPlayerEventHandlers();
            bool result = _taskInitialized.WaitOne(2000);
            //Send message to initiate playback
            if (result)
            {
               /* var message = new ValueSet();
                message.Add(Constants.StartPlayback, "0");
                BackgroundMediaPlayer.SendMessageToBackground(message);*/
            }
            else
            {
                throw new Exception("Background Audio Task didn't start in expected time");
            }
        }

        public void PlayTrack(string id)
        {
            _playlistManager.PlayAllTracksM();
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
                        _taskInitialized.Set();
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
