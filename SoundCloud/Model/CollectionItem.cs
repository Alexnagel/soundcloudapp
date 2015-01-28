using System.Runtime.Serialization;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace SoundCloud.Model
{
    [DataContract]
    public class CollectionItem
    {
        [ForeignKey(typeof(ScStream))]
        [IgnoreDataMember]
        public int StreamId { get; set; }

        [PrimaryKey, AutoIncrement]
        [IgnoreDataMember]
        public int Pid { get; set; }


        [DataMember(Name = "uuid")]
        public string Id { get; set; }

        [DataMember(Name="type")]
        public string Type { get; set; }

        [Ignore]
        [IgnoreDataMember]
        public Visibility IsRepost
        {
            get { return ((Type == "track-repost" || Type == "playlist-repost") ? Visibility.Visible : Visibility.Collapsed); }
        }

        [ForeignKey(typeof(User))]
        [IgnoreDataMember]
        public int UserId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [DataMember(Name = "user")]
        public User ItemUser { get; set; }

        [ForeignKey(typeof(Track))]
        [IgnoreDataMember]
        public int TrackId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [DataMember(Name = "track", IsRequired = false)]
        public Track ItemTrack { get; set; }

        [ForeignKey(typeof(Playlist))]
        [IgnoreDataMember]
        public int PlaylistId { get; set; }

        [DataMember(Name = "playlist", IsRequired = false)]
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public Playlist ItemPlaylist { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }
    }
}
