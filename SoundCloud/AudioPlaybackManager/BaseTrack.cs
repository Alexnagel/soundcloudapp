using System;

namespace AudioPlaybackManager
{
    public sealed class BaseTrack
    {
        public string Id { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public Uri PlaybackUri { get; set; }
    }
}
