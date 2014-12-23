using System;
using SQLite.Net.Attributes;


namespace BackgroundAudio.PlayQueue
{

    public sealed class BaseTrack
    {
        [PrimaryKey, AutoIncrement]
        public int dbId { get; set; }
        public string Id { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string ArtworkUri { get; set; }

        [Default(true, false)]
        public bool IsPlaying { get; set; }
        [Ignore]
        public Uri PlaybackUri
        {
            get
            {
                if (_playbackUri == null && !string.IsNullOrEmpty(PlaybackString))
                    _playbackUri = new Uri(_playbackString);
                return _playbackUri;
            }
            set { _playbackUri = value; }
        }

        public string PlaybackString
        {
            get
            {
                if (string.IsNullOrEmpty(_playbackString))
                    _playbackString = PlaybackUri.ToString();
                return _playbackString;
            }
            set { _playbackString = value; }
        }

        private Uri _playbackUri;
        private string _playbackString;
    }
}
