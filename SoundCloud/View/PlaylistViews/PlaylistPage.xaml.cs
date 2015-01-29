using System.Collections.Generic;
using System.ComponentModel;
using BackgroundAudio.PlayQueue;
using SoundCloud.Common;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoundCloud.Controller;
using SoundCloud.Model;

namespace SoundCloud.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistPage : Page
    {
        private NavigationHelper navigationHelper;
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public CollectionItem CollectionItem
        {
            get
            {
                return _collectionItem;
            }
            set
            {
                if (_collectionItem != value)
                {
                    _collectionItem = value;
                    NotifyPropertyChanged("CollectionItem");
                }
            }
        }

        public Playlist Playlist
        {
            get { return _playlist; }
            set
            {
                if (_playlist != value)
                {
                    _playlist = value;
                    NotifyPropertyChanged("Playlist");
                }
            }
        }

        public QueueType QueueType
        {
            get { return _queueType; }
            set
            {
                if (_queueType != value)
                {
                    _queueType = value;
                    NotifyPropertyChanged("QueueType");
                }
            }
        }

        private CollectionItem _collectionItem;
        private Playlist _playlist;
        private AppController _appController;
        private QueueType _queueType; 

        public RelayCommand ToStreamCommand { get; private set; }

        public PlaylistPage()
        {
            _appController = AppController.ControllerInstance;
            initRelayCommands();

            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        private void NavigateToStream()
        {
            _appController.NavigateToPage(typeof(StreamPage));
        }

        public void initRelayCommands()
        {
            ToStreamCommand = new RelayCommand(NavigateToStream);
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

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                object param = e.Parameter;
                _queueType = QueueType.Playlist;
                if (param.GetType() == typeof (CollectionItem))
                {
                    CollectionItem = (CollectionItem) param;
                    Playlist = CollectionItem.ItemPlaylist;
                    _queueType = QueueType.Stream;
                }
                else if (param.GetType() == typeof (Playlist))
                {
                    Playlist = (Playlist) param;
                }

                _appController.AudioManager.EmptyPlaylist(_queueType);
                _appController.AudioManager.AddToPlaylist(Playlist, QueueType.Playlist);
            }

            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #endregion
    }
}
