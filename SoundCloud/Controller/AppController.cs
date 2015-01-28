using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SoundCloud.Audio;
using SoundCloud.Data;
using SoundCloud.Services;
using SoundCloud.Services.Authentication;
using SoundCloud.View;

namespace SoundCloud.Controller
{
    public class AppController
    {
        #region Singleton 

        private static AppController _appController;
        public static AppController ControllerInstance
        {
            get
            {
                if (_appController == null)
                    _appController = new AppController();
                return _appController;
            }
        }
        #endregion Singleton

        #region Properties

        public AudioManager AudioManager
        {
            get { return _audioManager; }
        }

        public DataManager DataManager
        {
            get { return _dataManager; }
        }

        public bool ShowNowPlaying
        {
            get { return _audioManager.HasTrackOpen; }
        }
        #endregion Properties

        #region Variables

        private readonly DataManager _dataManager;
        private readonly AudioManager _audioManager;

        #endregion Variables

        private AppController()
        {
            _dataManager = new DataManager();
            _dataManager.SetSoundCloudClient(new SoundCloudClient());

            _audioManager = new AudioManager();
        }

        #region Navigation

        public void NavigateToPage(Type pageType)
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(pageType);
        }

        public void NavigateToPage(Type pageType, object param)
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(pageType, param);
        }

        #endregion Navigation

        #region Authentication

        public async Task<bool> Authenticate(SoundCloudClient soundCloudClient)
        {
            // await the authentication
            AccessToken success = await soundCloudClient.Authenticate();

            if (success != null)
            {
                // Set the SoundCloudClient in the Controller
                await _dataManager.SetSoundCloudClient(soundCloudClient);
                _dataManager.SaveAccessToken();

                _audioManager.UserAuth = _dataManager.GetUserAuthKey();
                return true;
            }
            return false;
        }

        public async Task<bool> IsUserLoggedIn()
        {
            AccessToken token = await _dataManager.GetUserAccessToken();
            if (token != null)
            {
                await _dataManager.SetSoundCloudClient(new SoundCloudClient(token));
                _audioManager.UserAuth = _dataManager.GetUserAuthKey();
                return true;
            }
            return false;
        }

        #endregion Authentication
    }
}
