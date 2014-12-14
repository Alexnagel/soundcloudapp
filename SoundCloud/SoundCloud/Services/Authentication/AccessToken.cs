using System.ComponentModel;
using System.Runtime.Serialization;

namespace SoundCloud.Services.Authentication
{
    [DataContract]
    public class AccessToken : INotifyPropertyChanged
    {
        [IgnoreDataMember]
        public int TokenId
        {
            get { return _tokenId; }
            set
            {
                if (_tokenId != value)
                {
                    _tokenId = value;
                    NotifyPropertyChanged("TokenId");
                }
            }
        }
        private int _tokenId;

        /// <summary>
        /// The access token given from SoundCloud
        /// </summary>
        [DataMember(Name = "access_token")]
        public string Token
        {
            get { return _token; }
            set
            {
                if (_token != value)
                {
                    _token = value;
                    NotifyPropertyChanged("Token");
                }
            }
        }
        private string _token;

        /// <summary>
        /// Holds the time till the token expires
        /// </summary>
        [DataMember(Name = "expires_in")]
        public int ExpiresIn
        {
            get { return _expiresIn; }
            set
            {
                if (_expiresIn != value)
                {
                    _expiresIn = value;
                    NotifyPropertyChanged("ExpiresIn");
                }
            }
        }
        private int _expiresIn;

        /// <summary>
        /// Holds the scope type of the token
        /// </summary>
        [DataMember(Name = "scope")]
        public string Scope
        {
            get { return _scope; }
            set
            {
                if (_scope != value)
                {
                    _scope = value;
                    NotifyPropertyChanged("Scope");
                }
            }
        }
        private string _scope;

        /// <summary>
        /// Holds the token to refresh the actual access token
        /// </summary>
        [DataMember(Name = "refresh_token")]
        public string RefreshToken
        {
            get { return _refreshToken; }
            set
            {
                if (_refreshToken != value)
                {
                    _refreshToken = value;
                    NotifyPropertyChanged("RefreshToken");
                }
            }
        }
        private string _refreshToken;


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
