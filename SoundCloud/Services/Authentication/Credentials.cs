namespace SoundCloud.Services.Authentication
{
    public class Credentials
    {
        private const string _clientId = "5ea213bbb33d27b0cb746f2ddf349342";
        private const string _clientSecret = "a6441c7f70eb6fe9b6d78a6dbe2650d0";

        #region Public Properties

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientID { get { return _clientId; } }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        public string ClientSecret { get { return _clientSecret; } }

        /// <summary>
        /// Gets or sets the authorization end point.
        /// </summary>
        public string EndUserAuthorization { get; set; }

        /// <summary>
        /// Gets or sets the user name required for authentication.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password required for authentication.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Get OAuth version.
        /// </summary>
        public double OAuth { get { return 2.0; } }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Credentials"/> class.
        /// </summary>
        public Credentials() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Credentials"/> class.
        /// </summary>
        /// 
        /// <param name="clientID"></param>
        /// <param name="clientSecret"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public Credentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        #endregion Constructors
    }
}
