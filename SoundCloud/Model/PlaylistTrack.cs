using SQLiteNetExtensions.Attributes;

namespace SoundCloud.Model
{
    public class PlaylistTrack
    {
        [ForeignKey(typeof(Playlist))]
        public int PlaylistId { get; set; }

        [ForeignKey(typeof(Track))]
        public int TrackId { get; set; }
    }
}
