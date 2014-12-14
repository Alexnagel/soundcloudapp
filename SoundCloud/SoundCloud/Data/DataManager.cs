using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using SoundCloud.Interfaces;
using SoundCloud.Services;
using SoundCloud.Services.Authentication;
using SoundCloud.Services.Utils;

namespace SoundCloud.Data
{
    class DataManager : IDataManager
    {
        private const string ACCESSTOKEN_FILE = "access_token.json";
        private static StorageFolder _settingsFolder;

        private SoundCloudClient _soundCloudClient;

        public DataManager()
        {
            _settingsFolder = ApplicationData.Current.LocalFolder;
        }

        public void SetSoundCloudClient(SoundCloudClient client)
        {
            _soundCloudClient = client;
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
    }
}
