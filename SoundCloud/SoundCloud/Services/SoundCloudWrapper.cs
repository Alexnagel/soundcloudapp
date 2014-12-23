using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SoundCloud.Exceptions;
using SoundCloud.Services.Authentication;
using SoundCloud.Services.Enums;
using SoundCloud.Services.Events;
using SoundCloud.Services.Utils;
using HttpMethod = SoundCloud.Services.Enums.HttpMethod;

namespace SoundCloud.Services
{
    /// <summary>
    /// Handles the Api calls
    /// </summary>
    internal class SoundCloudWrapper : SoundCloudClient
    {
        /// <summary>
        /// Dictionary containing the possible api calls and their corresponding URI
        /// </summary>
        private static readonly Dictionary<ApiCall, Uri> ApiDictionary = new Dictionary<ApiCall, Uri>()
        {
            //Authorization
            { ApiCall.AuthorizationFlow,    new Uri("https://soundcloud.com/connect?scope=non-expiring&client_id={0}&response_type={1}&redirect_uri={2}")},
            { ApiCall.UserAgentFlow,        new Uri("https://soundcloud.com/connect?client_id={0}&response_type=token&redirect_uri={1}") },
            { ApiCall.UserCredentialsFlow,  new Uri("https://api.soundcloud.com/oauth2/token?client_id={0}&client_secret={1}&grant_type=password&username={2}&password={3}") },
            { ApiCall.RefreshToken,         new Uri("https://api.soundcloud.com/oauth2/token?client_id={0}&client_secret={1}&grant_type=refresh_token&refresh_token={2}") },

            //Logged in User
            { ApiCall.Me,                   new Uri("https://api.soundcloud.com/me.json")} ,
            { ApiCall.MeStream,             new Uri("https://api-v2.soundcloud.com/stream")},
            { ApiCall.MeTracks,             new Uri("https://api.soundcloud.com/me/tracks.json") },
            { ApiCall.MeComments,           new Uri("https://api.soundcloud.com/me/comments.json") },
            { ApiCall.MeFollowing,          new Uri("https://api.soundcloud.com/me/followings.json") },
            { ApiCall.MeFollowers,          new Uri("https://api.soundcloud.com/me/followers.json") },
            { ApiCall.MeFavorites,          new Uri("https://api.soundcloud.com/me/favorites.json") },
            { ApiCall.MeFavoritesTrack,     new Uri("https://api.soundcloud.com/me/favorites/{0}.json") },
            { ApiCall.MeGroups,             new Uri("https://api.soundcloud.com/me/groups.json") },
            { ApiCall.MePlaylists,          new Uri("https://api.soundcloud.com/me/playlists.json") },
            { ApiCall.MeConnections,        new Uri("https://api.soundcloud.com/me/connections.json") },

            //Users
            { ApiCall.Users,                new Uri("https://api.soundcloud.com/users.json") },
            { ApiCall.User,                 new Uri("https://api.soundcloud.com/users/{0}.json") },
            { ApiCall.UserTracks,           new Uri("https://api.soundcloud.com/users/{0}/tracks.json") },
            { ApiCall.UserComments,         new Uri("https://api.soundcloud.com/users/{0}/comments.json") },
            { ApiCall.UserFollowing,        new Uri("https://api.soundcloud.com/users/{0}/followings.json") },
            { ApiCall.UserFollowers,        new Uri("https://api.soundcloud.com/users/{0}/followers.json") },
            { ApiCall.UserFavorites,        new Uri("https://api.soundcloud.com/users/{0}/favorites.json") },
            { ApiCall.UserFavoritesTrack,   new Uri("https://api.soundcloud.com/users/{0}/favorites/{1}.json") },
            { ApiCall.UserGroups,           new Uri("https://api.soundcloud.com/users/{0}/groups.json") },
            { ApiCall.UserPlaylists,        new Uri("https://api.soundcloud.com/users/{0}/playlists.json") },
            { ApiCall.UserFollowingContact, new Uri("https://api.soundcloud.com/users/{0}/followings/{contact_id}.json") },

            //Tracks
            { ApiCall.Tracks,               new Uri("https://api.soundcloud.com/tracks.json") },
            { ApiCall.Track,                new Uri("https://api.soundcloud.com/tracks/{0}.json") },
            { ApiCall.TrackComments,        new Uri("https://api.soundcloud.com/tracks/{0}/comments.json") },
            { ApiCall.TrackPermissions,     new Uri("https://api.soundcloud.com/tracks/{0}/permissions.json") },
            { ApiCall.TrackSecretToken,     new Uri("https://api.soundcloud.com/tracks/{0}/secret-token.json") },
            { ApiCall.TrackShare,           new Uri("https://api.soundcloud.com/tracks/{0}/shared-to/connections") },

            //Comments
            { ApiCall.Comment,              new Uri("https://api.soundcloud.com/comments/{0}.json") },

            //Groups
            { ApiCall.Groups,               new Uri("https://api.soundcloud.com/groups.json") },
            { ApiCall.Group,                new Uri("https://api.soundcloud.com/groups/{0}.json") },
            { ApiCall.GroupUsers,           new Uri("https://api.soundcloud.com/groups/{0}/users.json") },
            { ApiCall.GroupModerators,      new Uri("https://api.soundcloud.com/groups/{0}/moderators.json") },
            { ApiCall.GroupMembers,         new Uri("https://api.soundcloud.com/groups/{0}/members.json") },
            { ApiCall.GroupContributors,    new Uri("https://api.soundcloud.com/groups/{0}/contributors.json") },
            { ApiCall.GroupTracks,          new Uri("https://api.soundcloud.com/groups/{0}/tracks.json") },

            //Playlists
            { ApiCall.Playlists,            new Uri("https://api.soundcloud.com/playlists.json") },
            { ApiCall.Playlist,             new Uri("https://api.soundcloud.com/playlists/{0}.json") },

            //Resolver
            { ApiCall.Resolve,              new Uri("https://api.soundcloud.com/resolve.json?url={0}") }
        };

        #region Shared Methods

        /// <summary>
        /// Executes an api action.
        /// </summary>
        /// 
        /// <param name="command">Api command.</param>
        /// <param name="parameters">Parameters format of an api command uri.</param>
        public static async Task<T> ApiAction<T>(ApiCall command, params object[] parameters)
        {
            var uri =
                ApiDictionary[command].With(parameters);

            return await ApiAction<T>(uri);
        }


        /// <summary>
        /// Executes an api action.
        /// </summary>
        /// 
        /// <param name="command">Api command.</param>
        /// <param name="method">Http method. <seealso cref="Enums.HttpMethod"/>.</param>
        /// <param name="parameters">Parameters format of an api command uri.</param>
        public static async Task<T> ApiAction<T>(ApiCall command, HttpMethod method, params object[] parameters)
        {
            var uri =
                ApiDictionary[command]
                    .With(parameters);

            bool requireAuthentication = true;
            if (command == ApiCall.RefreshToken || command == ApiCall.UserCredentialsFlow)
            {
                requireAuthentication = false;
            }

            return await ApiAction<T>(uri, method, requireAuthentication);
        }

        /// <summary>
        /// Executes an api action.
        /// </summary>
        /// 
        /// <param name="command">Api command;</param>
        /// <param name="extraParameters">Dictionnary of parameters to be passed in the api action uri.</param>
        public static async Task<T> ApiAction<T>(ApiCall command, Dictionary<string, object> extraParameters)
        {
            var uri =
                ApiDictionary[command]
                    .UriAppendingParameters(extraParameters);


            return await ApiAction<T>(uri);
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="command"></param>
        /// <param name="method"></param>
        /// <param name="extraParameters"></param>
        /// <param name="parameters"></param>
        public static async Task<T> ApiAction<T>(ApiCall command, HttpMethod method, Dictionary<string, object> extraParameters, params object[] parameters)
        {
            var uri =
                ApiDictionary[command]
                    .UriAppendingParameters(extraParameters)
                    .With(parameters);

            return await ApiAction<T>(uri, method);
        }

        /// <summary>
        /// Executes an api action.
        /// </summary>
        /// 
        /// <param name="uri">Uri of the api command</param>
        /// <param name="method">Http method. <seealso cref="HttpMethod"/>.</param>
        /// <param name="requireAuthentication">The action requires an authentication or not.</param>
        /// <returns>An object returned back from the api action.</returns>
        public static async Task<T> ApiAction<T>(Uri uri, HttpMethod method = HttpMethod.GET, bool requireAuthentication = true)
        {
            Uri api = uri;

            if (requireAuthentication)
            {
                if (ScAccessToken == null)
                {
                    if (string.IsNullOrEmpty(ClientID))
                        throw new SoundCloudException(
                            "Please authenticate using the SoundCloudClient class contructor before making an API call");
                    // try an unauthenticated request
                    api = uri.UriWithClientID(ClientID);
                }
                else
                {
                    api = uri.UriWithAuthorizedUri(ScAccessToken.Token);
                }
            }

            // the request
            var request = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
                                            | DecompressionMethods.Deflate
            });

            HttpResponseMessage response = null;
            try
            {
                //OnApiActionExecuting Event
                OnApiActionExecuting(EventArgs.Empty);

                request.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate");

                switch (method)
                {
                    case HttpMethod.POST:
                        FormUrlEncodedContent strc = new FormUrlEncodedContent(Util.UriToFormData(api));
                        response = await request.PostAsync(api.ToString().Split('?')[0], strc);
                        break;
                    case HttpMethod.GET: response = await request.GetAsync(api);
                        break;
                }

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var args = new SoundCloudEventArgs { RawResponse = json, ReturnedType = typeof(T) };

                    //OnApiActionExecuted Event
                    OnApiActionExecuted(args);

                    return JsonSerializer.Deserialize<T>(json);
                }
                else
                {
                    //OnApiActionError Event

                    if (response.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        OnApiActionError(new SoundCloudEventArgs());
                        throw new SoundCloudException(string.Format("{0} : {1}", response.StatusCode,
                            response.ReasonPhrase));
                    }
                    else
                    {
                        AccessToken token =
                            await
                                ApiAction<AccessToken>(ApiCall.RefreshToken, HttpMethod.POST, ClientID,
                                    ClientSecret, ScAccessToken.RefreshToken);
                        if (token != null)
                        {
                            ScAccessToken = token;
                            
                            return await ApiAction<T>(uri, method, requireAuthentication);
                        }
                    }
                }
            }

            catch (SoundCloudException e)
            {
                throw new SoundCloudException(string.Format("{0} : {1}", response.StatusCode, response.ReasonPhrase));
            }
            return default(T);
        }
        #endregion Shared Methods
    }
}
