// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using SoundCloud.Audio;
using SoundCloud.Data;
using SoundCloud.Model;
using SoundCloud.Services.Events;

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
        private StreamLoadingCollection _streamTracks;
        private DataManager _dataManager;
        private AudioManager _audioManager;
        #endregion Variables

        public StreamPivot()
        {
            DataContext = this;
            initRelayCommands();

            _dataManager = DataManager.ManagerInstance;
            _streamTracks = new StreamLoadingCollection(_dataManager);

            _audioManager = AudioManager.ManagerInstance;
            _audioManager.UserAuth = _dataManager.GetUserAuthKey();

            _audioManager.SetPlaylist(_streamTracks);
            _streamTracks.CollectionChanged += _streamTracks_CollectionChanged;

            this.InitializeComponent();
            NotifyPropertyChanged("StreamTracks");
        }

        void _streamTracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _audioManager.SetPlaylist(_streamTracks);
            if (_streamTracks.Count == 1)
                _audioManager.PlayTrack(_streamTracks[0].Id);
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
