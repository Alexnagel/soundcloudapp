using System.ComponentModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using SoundCloud.Audio;
using SoundCloud.Common;
using SoundCloud.Controller;
using SoundCloud.Interfaces;
using SoundCloud.Model;

namespace SoundCloud.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StreamPage : Page, INotifyPropertyChanged
    {
        #region RelayCommands
        public RelayCommand ToNowPlayingCommand;
        #endregion RelayCommands

        #region Properties

        public AudioManager AudioManager
        {
            get { return _appController.AudioManager; }
        }

        public Visibility ShowNowPlaying
        {
            get { return _showNowPlaying; }
            set
            {
                if (_showNowPlaying != value)
                {
                    _showNowPlaying = value;
                    NotifyPropertyChanged("ShowNowPlaying");
                }
            }
        }

        #endregion Properties

        #region Variables
        private AppController _appController;
        private Visibility _showNowPlaying;
        #endregion Variables

        public StreamPage()
        {
            _appController = AppController.ControllerInstance;
            initRelayCommands();

            this.InitializeComponent();
            ShowNowPlaying = AudioManager.IsPlaying ? Visibility.Visible : Visibility.Collapsed;
            //ShowNowPlaying = Visibility.Visible;
            AudioManager.TrackLoaded += TrackLoaded;
        }

        private void TrackLoaded(string id)
        {
            _showNowPlaying = Visibility.Visible;
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => NotifyPropertyChanged("ShowNowPlaying"));
        }

        private void NavigateToNowPlaying()
        {
            _appController.NavigateToPage(typeof(NowPlayingPage));
        }

        private void initRelayCommands()
        {
            ToNowPlayingCommand = new RelayCommand(NavigateToNowPlaying);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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
