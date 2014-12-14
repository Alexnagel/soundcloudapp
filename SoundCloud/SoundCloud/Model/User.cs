using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using SoundCloud.Services;
using SoundCloud.Services.Enums;

namespace SoundCloud.Model
{
    /// <summary>
    /// SoundCloud user.
    /// </summary>
    [DataContract]
    public class User : SoundCloudClient
    {
        #region Properties

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "permalink")]
        public string Permalink { get; set; }

        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "permalink_url")]
        public string PermalinkUrl { get; set; }

        [DataMember(Name = "avatar_url")]
        public string Avatar { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "full_name")]
        public string FullName { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "discogs_name")]
        public string Discogs { get; set; }

        [DataMember(Name = "myspace_name")]
        public string Myspace { get; set; }

        [DataMember(Name = "website")]
        public string Website { get; set; }

        [DataMember(Name = "website_title")]
        public string WebsiteTitle { get; set; }

        [DataMember(Name = "online")]
        public bool? IsOnline { get; set; }

        [DataMember(Name = "track_count")]
        public int Tracks { get; set; }

        [DataMember(Name = "playlist_count")]
        public int Playlists { get; set; }

        [DataMember(Name = "followers_count")]
        public int Followers { get; set; }

        [DataMember(Name = "followings_count")]
        public int Followings { get; set; }

        [DataMember(Name = "public_favorites_count")]
        public int Favorites { get; set; }

        [DataMember(Name = "plan")]
        public string Plan { get; set; }

        [DataMember(Name = "private_tracks_count")]
        public int PrivateTracks { get; set; }

        [DataMember(Name = "private_playlists_count")]
        public int PrivatePlaylists { get; set; }

        [DataMember(Name = "primary_email_confirmed")]
        public bool EmailConfirmed { get; set; }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Returns a collection of users the user with user id is following.
        /// </summary>
        public async Task<List<User>> GetFollowings()
        {
            return await SoundCloudWrapper.ApiAction<List<User>>(ApiCall.UserFollowing, Id);
        }

        /// <summary>
        /// Adds the user with the id contact_id to the givens user's list of contacts.
        /// </summary>
        /// 
        /// <param name="user">User to follow.</param>
        public void AddFollowing(User user)
        {
            AddFollowing(user.Id);
        }

        /// <summary>
        /// Adds the user with the id contact_id to the givens user's list of contacts.
        /// </summary>
        /// 
        /// <param name="id">User id to follow.</param>
        public void AddFollowing(int id)
        {
            SoundCloudWrapper.ApiAction<User>(ApiCall.UserFollowingContact, HttpMethod.PUT, Id, id);
        }

        /// <summary>
        /// Removes the user with the id contact_id from the givens user's list of contacts.
        /// </summary>
        /// 
        /// <param name="user">User to remove.</param>
        public void RemoveFollowing(User user)
        {
            RemoveFollowing(user.Id);
        }

        /// <summary>
        /// Removes the user with the id contact_id from the givens user's list of contacts.
        /// </summary>
        /// 
        /// <param name="id">User id to remove.</param>
        public void RemoveFollowing(int id)
        {
            SoundCloudWrapper.ApiAction<User>(ApiCall.UserFollowingContact, HttpMethod.DELETE, Id, id);
        }

        /// <summary>
        /// Returns a collection of users who follow the user with user id
        /// </summary>
        public async Task<List<User>> GetFollowers()
        {
            return await SoundCloudWrapper.ApiAction<List<User>>(ApiCall.UserFollowers, Id);
        }

        /// <summary>
        /// Returns a collection of tracks uploaded by user id.
        /// </summary>
        public async Task<List<Track>> GetTracks()
        {
            return await SoundCloudWrapper.ApiAction<List<Track>>(ApiCall.UserTracks, Id);
        }

        /// <summary>
        /// Returns a collection of tracks favorited by the user with user id.
        /// </summary>
        public async Task<List<Track>> GetFavorites()
        {
            return await SoundCloudWrapper.ApiAction<List<Track>>(ApiCall.UserFavorites, Id);
        }

/* Comment Start
        /// <summary>
        /// Returns a collection of groups joined by user with user id.
        /// </summary>
        public List<Group> GetGroups()
        {
            return SoundCloudWrapper.ApiAction<List<Group>>(ApiCall.UserGroups, Id);
        }

        /// <summary>
        /// Returns a collection of playlists created by user with user id
        /// </summary>
        public async Task<List<Playlist>> GetPlaylists(int id)
        {
            return await SoundCloudWrapper.ApiAction<List<Playlist>>(ApiCall.UserPlaylists, Id);
        }
*/

        #endregion Public Methods

        #region Shared Methods

        /// <summary>
        /// Returns a collection of users.
        /// </summary>
        public static async Task<List<User>> GetAllUsers()
        {
            return await SoundCloudWrapper.ApiAction<List<User>>(ApiCall.Users);
        }

        /// <summary>
        /// Returns a limited collection of users.
        /// </summary>
        /// 
        /// <param name="count">Users count.</param>
        public static async Task<List<User>> GetUsers(int count)
        {
            var parameters = new Dictionary<string, object>();

            parameters.Add("limit", count);

            return await SoundCloudWrapper.ApiAction<List<User>>(ApiCall.Users, parameters);
        }

        /// <summary>
        /// Returns a user by user id.
        /// </summary>
        /// 
        /// <param name="id">User id.</param>
        public static async Task<User> GetUser(int id)
        {
            return await SoundCloudWrapper.ApiAction<User>(ApiCall.User, id);
        }

        /// <summary>
        /// Returns the logged-in user.
        /// </summary>
        public static async Task<User> Me()
        {
            return await SoundCloudWrapper.ApiAction<User>(ApiCall.Me);
        }

        /// <summary>
        /// Returns a collection of users based on a pattern.
        /// </summary>
        /// 
        /// <param name="term">a term to search for.</param>
        public static async Task<List<User>> Search(string term)
        {
            var parameters = new Dictionary<string, object> { { "q", term } };

            return await SoundCloudWrapper.ApiAction<List<User>>(ApiCall.Users, parameters);
        }

        #endregion Shared Methods
    }
}