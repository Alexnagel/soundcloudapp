// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using System.Collections.Specialized;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using SoundCloud.Audio;
using SoundCloud.Controller;
using SoundCloud.Data;
using SoundCloud.Model;

namespace SoundCloud.View.StreamViews
{
    public sealed partial class StreamPivot : UserControl, INotifyPropertyChanged
    {
        #region Data binding
        #endregion Data binding

        #region Properties

        public StreamLoadingCollection StreamTracks
        {
            get { return _streamTracks; }
            set
            {
                if (_streamTracks != value)
                {
                    _streamTracks = value;
                    NotifyPropertyChanged("StreamTracks");
                }
            }
        }

        #endregion Properties

        #region Variables
        private AppController _appController;
        private AudioManager _audioManager;

        private StreamLoadingCollection _streamTracks;
        private readonly object _padlock = new object();
        #endregion Variables

        public StreamPivot()
        {
            DataContext = this;
            initRelayCommands();

            _appController = AppController.ControllerInstance;
            _audioManager = _appController.AudioManager;
            
            InitStream();

            this.InitializeComponent();
        }

        private void InitStream()
        {
            if (!_audioManager.IsPlaying)
            {
                // Empty the current playlist
                _audioManager.EmptyPlaylist();
            }

            // Start the stream collection
            _streamTracks = new StreamLoadingCollection(_appController.DataManager);
            _streamTracks.CollectionChanged += _streamTracks_CollectionChanged;
            NotifyPropertyChanged("StreamTracks");
        }

        void _streamTracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (_padlock)
            {
                _audioManager.AddToPlaylist(_streamTracks[_streamTracks.Count - 1]);
            }
        }

        private void initRelayCommands()
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
