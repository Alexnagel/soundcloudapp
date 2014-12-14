using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using SoundCloud.Model;

namespace SoundCloud.View
{
    public sealed partial class StreamPivot : UserControl, INotifyPropertyChanged
    {
        #region Data binding
        #endregion Data binding

        #region Properties

        public ObservableCollection<Track> StreamTracks
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
        private ObservableCollection<Track> _streamTracks;
        #endregion Variables

        public StreamPivot()
        {
            DataContext = this;
            this.InitializeComponent();
            loadStream();
        }

        private async void loadStream()
        {
            Track tracks = await Track.GetTrack(181209699);
            _streamTracks = new ObservableCollection<Track>();
            _streamTracks.Add(tracks);
            NotifyPropertyChanged("StreamTracks");
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
