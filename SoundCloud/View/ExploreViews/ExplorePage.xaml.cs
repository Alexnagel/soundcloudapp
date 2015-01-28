using SoundCloud.Audio;
using SoundCloud.Common;
using SoundCloud.Controller;
using SoundCloud.Data;
using SoundCloud.Model;
using SoundCloud.Model.Explore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace SoundCloud.View.ExploreViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExplorePage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        #region Variables

        private ExploreLoadingCollection _exploreTracks;
        private AppController _appController;
        private AudioManager _audioManager;

        private String chosenCategory;
        private readonly object _padlock = new object();

        #endregion Variables

        #region Properties

        public ExploreLoadingCollection CategoryTrack
        {
            get { return _exploreTracks; }
            set
            {
                if (_exploreTracks != value)
                {
                    _exploreTracks = value;
                    NotifyPropertyChanged("CategoryTrack");
                }
            }
        }

        public String CategoryTitle
        {
            get { return chosenCategory; }
            set
            {
                if(chosenCategory != value)
                {
                    chosenCategory = value;
                    NotifyPropertyChanged("CategoryTitle");
                }
            }
        }

        #endregion Properties

        public ExplorePage()
        {

            _appController = AppController.ControllerInstance;
            _audioManager = _appController.AudioManager;

            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        public async void InitExploreItems()
        {
            if (!_audioManager.IsPlaying)
                _audioManager.EmptyPlaylist();

            _exploreTracks = new ExploreLoadingCollection(_appController.DataManager, chosenCategory);
            //_exploreTracks.CollectionChanged += _exploreTracks_CollectionChanged;
            NotifyPropertyChanged("CategoryTrack");
        }

        private void _exploreTracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (_padlock)
            {
                //Todo audiomanager shit
                //_audioManager.AddCatTrack(_exploreTracks[_exploreTracks.Count - 1]);
            }
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

        #endregion INotifyPropertyChanged Members

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
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
            chosenCategory = (String) e.Parameter;
            CategoryTitle = chosenCategory;
            NotifyPropertyChanged("CategoryTitle");
            InitExploreItems();
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
