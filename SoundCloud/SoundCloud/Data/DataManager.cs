using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using SoundCloud.Interfaces;
using SoundCloud.Model;
using SoundCloud.Services;
using SoundCloud.Services.Authentication;
using SoundCloud.Services.Utils;

namespace SoundCloud.Data
{
    public class DataManager : IDataManager
    {
        #region SingletonStuff
        private static DataManager _managerInstance;

        public static DataManager ManagerInstance
        {
            get
            {
                if (_managerInstance == null)
                    _managerInstance = new DataManager();

                return _managerInstance;
            }
        }
        #endregion SingletonStuff

        #region Variables
        private const string ACCESSTOKEN_FILE = "access_token.json";
        private static StorageFolder _settingsFolder;

        private SoundCloudClient _soundCloudClient;

        private User _currentUser;
        private ScStream _stream;
        #endregion Variables

        #region Properties
        public User CurrentUser
        {
            get { return _currentUser; }
        }
        #endregion Properties

        private DataManager()
        {
            _settingsFolder = ApplicationData.Current.LocalFolder;
#if DEBUG
            AccessToken token = new AccessToken();
            token.ExpiresIn = 21599;
            token.Scope = "*";
            token.Token = "1-63622-2668798-9bf2b2907160f912";
            token.RefreshToken = "f8175be9cbffc54dbfadc82e9d7c438b";

            _soundCloudClient = new SoundCloudClient(token);
#endif
        }

        public void SetSoundCloudClient(SoundCloudClient client)
        {
            //_soundCloudClient = client;
            //setCurrentUser();
        }

        private async void setCurrentUser()
        {
            _currentUser = await User.Me();
        }

        #region Authentication

        public string GetUserAuthKey()
        {
            return _soundCloudClient.scAccessToken.Token;
        }

        public async Task<AccessToken> GetUserAccessToken()
        {
            try
            {
                StorageFile accesTokenFile = await _settingsFolder.GetFileAsync(ACCESSTOKEN_FILE);
                string jsonString = await FileIO.ReadTextAsync(accesTokenFile);
                return JsonSerializer.Deserialize<AccessToken>(jsonString);
            }
            catch (FileNotFoundException e)
            {
                return null;
            }
            catch (UnauthorizedAccessException e)
            {
                return null;
            }
            catch (ArgumentException e)
            {
                return null;
            }
        }

        public async void SaveAccessToken()
        {
            string jsonString = JsonSerializer.Serialize(_soundCloudClient.scAccessToken);

            bool fileExists = true;
            StorageFile accessTokenFile = null;
            try
            {
                accessTokenFile = await _settingsFolder.GetFileAsync(ACCESSTOKEN_FILE);
            }
            catch (FileNotFoundException e)
            {
                fileExists = false;
            }

            if (fileExists == false)
            {
                accessTokenFile = await _settingsFolder.CreateFileAsync(ACCESSTOKEN_FILE);
            }

            try
            {
                FileIO.WriteTextAsync(accessTokenFile, jsonString);
            }
            catch (FileNotFoundException e)
            {
                // catch exception
            }
        }
        #endregion Authentication

        #region Stream

        public async Task<ObservableCollection<CollectionItem>> GetStream()
        {
            if (_stream == null)
                _stream = await ScStream.GetStream();
            return _stream.StreamItems;
        }

        public async Task<ObservableCollection<CollectionItem>> GetNextPageStream()
        {
            if (_stream != null)
            {
                ScStream temp = await ScStream.GetNextStream(_stream.NextHref);

                ObservableCollection<CollectionItem> newItems = temp.StreamItems;
                temp.StreamItems = new ObservableCollection<CollectionItem>(temp.StreamItems.Concat(_stream.StreamItems));
                _stream = temp;

                // Return new items for prepending in the class using it
                return newItems;
            }
            else
                return await GetStream();
        }

        #endregion Stream
    }
}
