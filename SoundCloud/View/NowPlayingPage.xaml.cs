using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using BackgroundAudio.PlayQueue;
using SoundCloud.Audio;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoundCloud.Common;
using SoundCloud.Controller;
using SoundCloud.View.UserViews;

namespace SoundCloud.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlayingPage : Page, INotifyPropertyChanged
    {
        private NavigationHelper navigationHelper;
        private readonly AudioManager _audioManager;
        private AppController _appController;
        private bool _isPlaying;

        private DispatcherTimer _progressTimer;

        #region Properties
        public BaseTrack CurrentTrack
        {
            get { return _currentTrack; }
        }
        private BaseTrack _currentTrack;

        public Visibility ShowPlayPath
        {
            get { return _showPlayPath; }
            set
            {
                if (_showPlayPath != value)
                {
                    _showPlayPath = value;
                    NotifyPropertyChanged("ShowPlayPath");
                }
            }
        }
        private Visibility _showPlayPath;

        public Visibility ShowPausePath
        {
            get { return _showPausePath; }
            set
            {
                if (_showPausePath != value)
                {
                    _showPausePath = value;
                    NotifyPropertyChanged("ShowPausePath");
                }
            }
        }
        private Visibility _showPausePath;

        #endregion Properties

        #region ProgressBar

        private double _maximum;
        public Double Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;
                NotifyPropertyChanged("Maximum");
            }
        }

        private double _currentPosition;
        public double CurrentPosition
        {
            get { return _currentPosition; }
            set
            {
                _currentPosition = value;
                NotifyPropertyChanged("CurrentPosition");
            }
        }

        private string _currentDuration;
        public string CurrentDuration
        {
            get { return _currentDuration; }
            set
            {
                _currentDuration = value;
                NotifyPropertyChanged("CurrentDuration");
            }
        }

        private string _duration;
        public string Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                NotifyPropertyChanged("Duration");
            }
        }
        #endregion ProgressBar

        #region Relaycommands

        public RelayCommand ScrubChangeCommand { get; private set; }
        public RelayCommand NextTrackCommand { get; private set; }
        public RelayCommand PreviousTrackCommand { get; private set; }
        public RelayCommand PlayPauseCommand { get; private set; }
        public RelayCommand ToStreamCommand { get; private set; }
        public RelayCommand ToUserCommand { get; private set; }

        #endregion Relaycommands

        public NowPlayingPage()
        {
            _appController = AppController.ControllerInstance;
            _audioManager = _appController.AudioManager;
            _currentTrack = _audioManager.GetCurrentTrack();
            
            // Data binding
            initRelayCommands();

            // Initialize audiomanager handlers
            _audioManager.TrackChanged +=_audioManager_TrackChanged;
            _audioManager.PlayerStateChanged += _audioManager_PlayerStateChanged;

            // Initialize progressbar timer
            _progressTimer = new DispatcherTimer();
            _progressTimer.Interval = TimeSpan.FromSeconds(1);
            _progressTimer.Tick += TrackPositionChanged;

            // Default values
            _showPlayPath = Visibility.Collapsed;
            _showPausePath = Visibility.Visible;
            _isPlaying = true;
            this.InitializeComponent();
            SetProgressbar();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        private void _audioManager_PlayerStateChanged(bool isPlaying)
        {
            if (isPlaying)
            {
                _showPlayPath = Visibility.Collapsed;
                _showPausePath = Visibility.Visible;
                _isPlaying = true;
            }
            else
            {
                _showPlayPath = Visibility.Visible;
                _showPausePath = Visibility.Collapsed;
                _isPlaying = false;
            }

            // Notify property changed UI thread
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NotifyPropertyChanged("ShowPlayPath"); 
                NotifyPropertyChanged("ShowPausePath");
            });
        }

        private void _audioManager_TrackChanged(object sender, EventArgs e)
        {
            _currentTrack = _audioManager.GetCurrentTrack();

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _progressTimer.Stop();
                NotifyPropertyChanged("CurrentTrack");
            });

            SetProgressbar();
        }

        private void SetProgressbar()
        {
            // Set maximum duration string
            TimeSpan ts = TimeSpan.FromMilliseconds(_currentTrack.Duration);
            if (ts.TotalHours >= 1)
                _duration = ts.ToString(@"%h\:mm\:ss");
            else
                _duration = ts.ToString(@"mm\:ss");
            _currentDuration = "00:00";

            _maximum = ts.TotalSeconds;
            _currentPosition = 0;

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _progressTimer.Start();

                NotifyPropertyChanged("Maximum");
                NotifyPropertyChanged("CurrentPosition");
                NotifyPropertyChanged("CurrentDuration");
                NotifyPropertyChanged("Duration");
            });
        }

        private void TrackPositionChanged(object sender, object e)
        {
            TimeSpan ts = _audioManager.CurrentTrackPosition();
            if (ts.TotalHours >= 1)
                _currentDuration = ts.ToString(@"%hh\:mm\:ss");
            else
                _currentDuration = ts.ToString(@"mm\:ss");

            _currentPosition = ts.TotalSeconds;

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NotifyPropertyChanged("CurrentPosition");
                NotifyPropertyChanged("CurrentDuration");
            });
        }

        private void ScrubChange()
        {
            // stop the timer so scrubbing can happen
            _progressTimer.Stop();

            _audioManager.SeekInTrack(_currentPosition);
            // Start the timer again 
            _progressTimer.Start();
        }

        private void NextTrack()
        {
            Task.Run(() => _audioManager.NextTrack());
        }

        private void PreviousTrack()
        {
            Task.Run(() => _audioManager.PreviousTrack());
        }

        private void PlayPauseTrack()
        {
            if (_isPlaying)
                _audioManager.PauseTrack();
            else
                _audioManager.PlayCurrentTrack();
        }

        private void NavigateToStream()
        {
            _appController.NavigateToPage(typeof(StreamPage));
        }

        private void NavigateToUser()
        {
            _appController.NavigateToPage(typeof(UserPage), CurrentTrack.ArtistId);
        }

        private void initRelayCommands()
        {
            ScrubChangeCommand = new RelayCommand(ScrubChange);
            NextTrackCommand = new RelayCommand(NextTrack);
            PreviousTrackCommand = new RelayCommand(PreviousTrack);
            PlayPauseCommand = new RelayCommand(PlayPauseTrack);
            ToStreamCommand = new RelayCommand(NavigateToStream);
            ToUserCommand = new RelayCommand(NavigateToUser);
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

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
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
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
