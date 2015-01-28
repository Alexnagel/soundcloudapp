using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using SoundCloud.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using SoundCloud.Services.Enums;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace SoundCloud.Model
{
    [DataContract]
    public class Playlist : SoundCloudClient
    {
        #region Properties

        [DataMember(Name = "id")]
        [PrimaryKey]
        public int Id { get; set; }

        [DataMember(Name = "created_at")]
        private string _CreationDate;
        /// <summary>
        /// Gets or sets the comment's creation date.
        /// </summary>
        [Ignore]
        [IgnoreDataMember]
        public DateTime CreationDate { get { return (DateTime.Parse(_CreationDate)); } set { _CreationDate = value.ToString(CultureInfo.InvariantCulture); } }

        [DataMember(Name = "user_id")]
        [ForeignKey(typeof(User))]
        public int UserId { get; set; }

        [DataMember(Name = "user")]
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public User User { get; set; }

        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        [Ignore]
        [IgnoreDataMember]
        public string DurationString
        {
            get
            {
                if (_durationString == null)
                {
                    TimeSpan t = TimeSpan.FromMilliseconds(Duration);
                    if (t.TotalHours >= 1)
                        _durationString = t.ToString(@"%h\:mm\:ss");
                    else
                        _durationString = t.ToString(@"mm\:ss");
                }
                return _durationString;
            }
        }
        private string _durationString;

        [DataMember(Name = "sharing")]
        public string Sharing { get; set; }

        [DataMember(Name = "tag_list")]
        public string TagsList { get; set; }

        [DataMember(Name = "permalink")]
        public string Permalink { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "streamable")]
        public bool Streamabale { get; set; }

        [DataMember(Name = "downloadable")]
        public bool Downloadable { get; set; }

        [DataMember(Name = "genre")]
        public string Genre { get; set; }

        [DataMember(Name = "release")]
        public string Release { get; set; }

        [DataMember(Name = "purchase_url")]
        public string PurchaseUrl { get; set; }

        [DataMember(Name = "label_id")]
        public int? LabelId { get; set; }

        [DataMember(Name = "label_name")]
        public string LabelName { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "playlist_type")]
        public string PlaylistType { get; set; }

        [DataMember(Name = "ean")]
        public string Ean { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "release_year")]
        public int? ReleaseYear { get; set; }

        [DataMember(Name = "release_month")]
        public int? ReleaseMonth { get; set; }

        [DataMember(Name = "release_day")]
        public int? ReleaseDay { get; set; }

        [Ignore]
        [IgnoreDataMember]
        public DateTime RealeaseDate
        {
            get
            {
                if (ReleaseDay != null && ReleaseMonth != null && ReleaseYear != null)
                    return new DateTime((int)ReleaseYear, (int)ReleaseMonth, (int)ReleaseDay);

                return DateTime.MinValue;
            }
        }

        [DataMember(Name = "license")]
        public string License { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "permalink_url")]
        public string PermalinkUrl { get; set; }

        [DataMember(Name = "artwork_url")]
        public string ArtworkUrl { get; set; }

        [Ignore]
        [IgnoreDataMember]
        public BitmapImage ArtworkImage
        {
            get
            {
                if (_artworkImage == null)
                {
                    if (ArtworkUrl != null)
                        _artworkImage = new BitmapImage(new Uri(ArtworkUrl, UriKind.Absolute));
                    else
                        _artworkImage = new BitmapImage();
                }

                return _artworkImage;
            }
        }
        private BitmapImage _artworkImage;

        [DataMember(Name = "tracks")]
        [ManyToMany(typeof(PlaylistTrack), CascadeOperations = CascadeOperation.All)]
        public List<Track> Tracks { get; set; }

        private int _trackAmount;
        [IgnoreDataMember]
        public int TrackAmount { 
            get
            {
                if (_trackAmount == 0)
                    _trackAmount = Tracks.Count;
                return _trackAmount;
            }
            set { _trackAmount = value; }
        }

        #endregion Public

        #region Shared Methods

        /// <summary>
        /// Returns a collection of playlists the user has.
        /// </summary>
        public static Task<List<Playlist>> GetAllUserPlaylists()
        {
            return SoundCloudWrapper.ApiAction<List<Playlist>>(ApiCall.MePlaylists);
        }

        /// <summary>
        /// Returns a collection of playlists.
        /// </summary>
        public static Task<List<Playlist>> GetAllPlaylists()
        {
            return SoundCloudWrapper.ApiAction<List<Playlist>>(ApiCall.Playlists);
        }

        /// <summary>
        /// Returns a playlist by playlist id.
        /// </summary>
        /// 
        /// <param name="id">Playlist id.</param>
        public static Task<Playlist> GetPlaylist(int id)
        {
            return SoundCloudWrapper.ApiAction<Playlist>(ApiCall.Playlist, id);
        }

        #endregion Shared Methods
    }
}
