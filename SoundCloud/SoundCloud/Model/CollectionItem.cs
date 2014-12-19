using System.Runtime.Serialization;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;

namespace SoundCloud.Model
{
    [DataContract]
    public class CollectionItem
    {
        [DataMember(Name = "uuid")]
        public string Id { get; set; }

        [DataMember(Name="type")]
        public string Type { get; set; }

        public Visibility IsRepost
        {
            get { return Type == "track-repost" ? Visibility.Visible : Visibility.Collapsed; }
        }

        [DataMember(Name = "user")]
        public User ItemUser { get; set; }

        [DataMember(Name = "track", IsRequired = false)]
        public Track ItemTrack { get; set; }

        //[DataMember(Name = "playlist", IsRequired = false)]
        //public Playlist ItemPlaylist { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }
    }
}
