﻿using System;
using Windows.UI.Xaml.Media.Imaging;
using SQLite.Net.Attributes;

namespace BackgroundAudio.PlayQueue
{

    public sealed class BaseTrack
    {
        [PrimaryKey, AutoIncrement]
        public int dbId { get; set; }
        public string Id { get; set; }
        public string Artist { get; set; }
        public int ArtistId { get; set; }
        public string Title { get; set; }
        public string ArtworkUri { get; set; }
        public double Duration { get; set; }
        public QueueType Type { get; set; }


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

        [Ignore]
        public BitmapImage ArtworkImage
        {
            get
            {
                if (_artworkImage == null && !String.IsNullOrEmpty(ArtworkUri))
                    _artworkImage = new BitmapImage(new Uri(ArtworkUri));
                else
                    _artworkImage = new BitmapImage(new Uri(@"ms-appx://Assets/empty_image.jpg", UriKind.Absolute));
                return _artworkImage;
            }
        }

        [Ignore]
        public BitmapImage BigArtworkImage
        {
            get
            {
                if (_bigArtworkImage == null)
                {
                    if (String.IsNullOrEmpty(ArtworkUri))
                        return new BitmapImage();
                    else
                    {
                        string uri = ArtworkUri.Replace("large", "t500x500");
                        _bigArtworkImage = new BitmapImage(new Uri(uri));
                    }
                }
                return _bigArtworkImage;
            }
        }

        private BitmapImage _artworkImage;
        private BitmapImage _bigArtworkImage;
        private Uri _playbackUri;
        private string _playbackString;
    }
}
