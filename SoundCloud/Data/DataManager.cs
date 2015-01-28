using System;
using System.Collections.Generic;
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
using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;
using SQLiteNetExtensions.Extensions;

namespace SoundCloud.Data
{
    public class DataManager : IDataManager
    {
        #region Variables
        private const string DB_NAME = "SoundCloud.db";
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

        public DataManager()
        {
            _settingsFolder = ApplicationData.Current.LocalFolder;

            SQLite3.Config(ConfigOption.Serialized);
            InitDb();
        }

        #region Private DB Init
        private async void InitDb()
        {
            // Create Db if not exist
            bool dbExist = await CheckDbAsync();
            if (!dbExist)
            {
               CreateDatabase();
            }
        }

        private async Task<bool> CheckDbAsync()
        {
            bool dbExist = true;

            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(DB_NAME);
            }
            catch (Exception)
            {
                dbExist = false;
            }

            return dbExist;
        }

        private void CreateDatabase()
        {
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                dbConnection.CreateTable<ScStream>();
                dbConnection.CreateTable<CollectionItem>();
                dbConnection.CreateTable<User>();
                dbConnection.CreateTable<Track>();
                dbConnection.CreateTable<Playlist>();
                dbConnection.CreateTable<PlaylistTrack>();
            }
        }
        #endregion Private DB Init

        public void SetSoundCloudClient(SoundCloudClient client)
        {
            _soundCloudClient = client;
            if (client.IsAuthenticated)
                setCurrentUser();
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

        public bool EmptyStream()
        {
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                try
                {
                    return dbConnection.DeleteAll<ScStream>() == 1 && dbConnection.DeleteAll<CollectionItem>() == 1;
                }
                catch (SQLiteException e)
                {
                    // no ScStream table
                    return true;
                }
            }
        }

        public ScStream GetStreamFromDB()
        {
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                try
                {
                    return dbConnection.Table<ScStream>().First();
                }
                catch (InvalidOperationException e)
                {
                    // no ScStream table
                    return null;
                }
            }
        }

        public async Task<ScStream> GetStreamFromUri()
        {
            ScStream stream = await ScStream.GetStream();
            stream.LastUpdated = DateTime.Now;

            _stream = stream;
            SaveStream();
            return stream;
        }

        public async Task<ObservableCollection<CollectionItem>> GetStream()
        {
            if (_stream == null)
            {
                ScStream stream = GetStreamFromDB();
                if (stream != null && (DateTime.Now - stream.LastUpdated).TotalHours <= 8)
                {
                    stream.StreamItemList = GetNextPageStreamDb(-1).ToList();
                    _stream = stream;
                }
                else if (stream != null && (DateTime.Now - stream.LastUpdated).TotalHours > 8)
                {
                    EmptyStream();
                    await GetStreamFromUri();
                }
                else
                {
                    await GetStreamFromUri();
                }
            }
            return new ObservableCollection<CollectionItem>(_stream.StreamItemList);
        }

        public async Task<ObservableCollection<CollectionItem>> GetNextPageStream(int lastId)
        {
            if (_stream != null)
            {
                ObservableCollection<CollectionItem> nextPage = GetNextPageStreamDb(lastId);
                if (nextPage == null || nextPage.Count == 0)
                    return await GetNextPageStreamUri();
                else
                    return nextPage;
            }
            else
                return await GetStream();
        }

        private ObservableCollection<CollectionItem> GetNextPageStreamDb(int lastId)
        {
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                try
                {
                    if (lastId == -1 && dbConnection.Table<CollectionItem>().Any())
                    {
                        lastId = dbConnection.Table<CollectionItem>().First().Pid - 1;
                    }
                    else if (lastId == -1 && !dbConnection.Table<CollectionItem>().Any())
                    {
                        return null;
                    } 

                    var query = dbConnection.Query<CollectionItem>("select Pid from CollectionItem where Pid > ? order by CreatedAt DESC limit 10", lastId).ToList();

                    var streamItems = new List<CollectionItem>();
                    foreach (var collectionItem in query)
                    {
                        streamItems.Add(dbConnection.GetWithChildren<CollectionItem>(collectionItem.Pid, true));
                    }

                     return new ObservableCollection<CollectionItem>(streamItems);
                }
                catch (SQLiteException e)
                {
                    // no ScStream table
                    return null;
                }
            }
        }

        public async Task<ObservableCollection<CollectionItem>> GetNextPageStreamUri()
        {
            if (_stream != null)
            {
                ScStream temp = await ScStream.GetNextStream(_stream.NextHref);

                var newItems = new ObservableCollection<CollectionItem>(temp.StreamItemList);
                _stream.StreamItemList = new List<CollectionItem>(temp.StreamItemList.Concat(_stream.StreamItemList));
                _stream.NextHref = temp.NextHref;

                SaveStream();

                // Return new items for prepending in the class using it
                return newItems;
            }
            else
                return await GetStream();
        }

        private void SaveStream()
        {
            using (var dbConnection = new SQLiteConnection(new SQLitePlatformWinRT(), DB_NAME))
            {
                try
                {
                    if (_stream != null)
                        dbConnection.InsertOrReplaceWithChildren(_stream, true);
                }
                catch (SQLiteException e)
                {
                }
            }
        }

        #endregion Stream

        #region Playlist

        public async Task<List<Playlist>> GetUserPlaylists()
        {
            return await Playlist.GetAllUserPlaylists();
        }

        public async Task<List<Playlist>> GetPlaylists()
        {
            return await Playlist.GetAllPlaylists();
        }

        public async Task<Playlist> GetPlaylist(int id)
        {
            return await Playlist.GetPlaylist(id);
        }

        #endregion Playlist
    }
}
