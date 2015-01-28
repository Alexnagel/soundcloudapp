using System;
using System.ComponentModel;
using Windows.ApplicationModel.Store;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641
using SoundCloud.Controller;
using SoundCloud.Interfaces;
using SoundCloud.Services;
using SoundCloud.Services.Authentication;
using SoundCloud.Services.Events;

namespace SoundCloud.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page, INotifyPropertyChanged
    {
        #region Data Binding
        public RelayCommand LoginClickCommand { get; private set; }
        public RelayCommand ForgotClickCommand { get; private set; }
        #endregion Data Binding

        #region Properties

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    NotifyPropertyChanged("Username");
                }
            }
        }

        public string LoginString
        {
            get { return _loginString; }
            set
            {
                _loginString = value;
                NotifyPropertyChanged("LoginString");
            }
        }

        public Visibility ProgressbarVisibility
        {
            get { return _progressbarVisibility; }
            set
            {
                if (_progressbarVisibility != value)
                {
                    _progressbarVisibility = value;
                    NotifyPropertyChanged("ProgressbarVisibility");
                }
            }
        }
        #endregion Properties

        #region Variables
        private AppController _appController;
        private string _username;
        private string _loginString;

        // Default progressbar 
        private Visibility _progressbarVisibility = Visibility.Collapsed;
        #endregion Variables

        public LoginPage()
        {
            initRelayCommands();
            _appController = AppController.ControllerInstance;

            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewParams viewParams = (ViewParams) e.Parameter;
        }

        private async void AuthenticateUser(SoundCloudClient soundCloudClient)
        {
            bool success = await _appController.Authenticate(soundCloudClient);

            // hide progress indicator
            ProgressbarVisibility = Visibility.Collapsed;

            if (success)
                _appController.NavigateToPage(typeof(StreamPage));
            else
            {
                MessageDialog dialog = new MessageDialog("Unable to login, please try again.");
                dialog.ShowAsync();
            }
        }

        private async void Login(object commandParam)
        {
            var passwordBox = commandParam as PasswordBox;

            if (String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(passwordBox.Password))
            {
                MessageDialog dialog = new MessageDialog("Please fill in all your credentials");
                await dialog.ShowAsync();
                return;
            }

            Credentials creds = new Credentials(Username, passwordBox.Password);
            SoundCloudClient soundCloudClient = new SoundCloudClient(creds);

            // Set progress indicator
            ProgressbarVisibility = Visibility.Visible;

            AuthenticateUser(soundCloudClient);
        }

        private void ForgotLogin(object commandParam)
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri("https://soundcloud.com/login/forgot"));
        }

        private void initRelayCommands()
        {
            LoginClickCommand = new RelayCommand(Login);
            ForgotClickCommand = new RelayCommand(ForgotLogin);
        }

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
