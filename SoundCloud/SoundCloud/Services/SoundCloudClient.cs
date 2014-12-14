using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using SoundCloud.Exceptions;
using SoundCloud.Services.Authentication;
using SoundCloud.Services.Enums;
using SoundCloud.Services.Events;

namespace SoundCloud.Services
{
    [DataContract]
    public class SoundCloudClient
    {
        /// <summary>
        /// Property for the user credentials that are needed for authentication
        /// </summary>
        protected Credentials Credentials { get; set; }

        /// <summary>
        /// Shows if the user is authenticated or not.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        public AccessToken scAccessToken
        {
            get { return ScAccessToken; }
        }

        /// <summary>
        /// The access token for the user
        /// </summary>
        protected static AccessToken ScAccessToken = null;

        /// <summary>
        /// Should GZip be used in the data
        /// </summary>
        protected static bool EnableGZip = false;

        /// <summary>
        /// The client id given
        /// </summary>
        protected static string ClientID = "5ea213bbb33d27b0cb746f2ddf349342";

        public SoundCloudClient()
        {
            EnableGZip = true;
        }

        /// <summary>
        /// Initializes a new isntance of the <see cref="SoundCloudClient"/> class.
        /// </summary>
        /// 
        /// <param name="credentials">Required credentials for authentication.</param>
        public SoundCloudClient(Credentials credentials)
        {
            Credentials = credentials;

            EnableGZip = true;
        }

        /// <summary>
        /// Initializes a new isntance of the <see cref="SoundCloudClient"/> class.
        /// </summary>
        /// 
        /// <param name="accessToken">Required AccessToken for authentication.</param>
        public SoundCloudClient(AccessToken accessToken)
        {
            ScAccessToken = accessToken;

            EnableGZip = true;
        }

        /// <summary>
        /// Authenticate the user with the given credentials
        /// </summary>
        /// <returns></returns>
        public async Task<AccessToken> Authenticate()
        {
            AccessToken token = null;

            try
            {
                token = await SoundCloudWrapper.ApiAction<AccessToken>(ApiCall.UserCredentialsFlow, HttpMethod.POST,
                    Credentials.ClientID, Credentials.ClientSecret, Credentials.UserName, Credentials.Password);
            }
            catch (SoundCloudException e)
            {
                // catch the exception to be more clear to the user
            }

            ScAccessToken = token;

            if (token != null)
            {
                IsAuthenticated = true;

                // some sort of security
                Credentials.Password = "";
            }
            return token;
        }

        #region defaultHandlers
        public delegate void EventHandler(object sender, EventArgs e);
        public static event EventHandler ApiActionExecuting;

        protected static void OnApiActionExecuting(EventArgs e)
        {
            if (ApiActionExecuting != null)
                ApiActionExecuting(null, e);
        }

        public static event EventHandler ApiActionExecuted;

        protected static void OnApiActionExecuted(SoundCloudEventArgs e)
        {
            if (ApiActionExecuted != null)
                ApiActionExecuted(null, e);
        }

        public static event EventHandler ApiActionError;

        protected static void OnApiActionError(EventArgs e)
        {
            if (ApiActionError != null)
                ApiActionError(null, e);
        }
        #endregion defaultHandlers
    }
}
