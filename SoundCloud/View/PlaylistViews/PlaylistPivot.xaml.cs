using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using SoundCloud.Controller;
using SoundCloud.Model;

namespace SoundCloud.View
{
    public sealed partial class PlaylistPivot : UserControl, INotifyPropertyChanged
    {

        #region Databinding

        #endregion Databinding

        #region Properties

        public ObservableCollection<Playlist> Playlists
        {
            get { return _playlists; }
            set
            {
                if (_playlists != value)
                {
                    _playlists = value;
                    NotifyPropertyChanged("Playlists");
                }
            }
        }

        #endregion Properties

        #region variables

        private ObservableCollection<Playlist> _playlists;
        private AppController _appController;
        #endregion variables

        public PlaylistPivot()
        {
            _appController = AppController.ControllerInstance;
            initRelayCommands();

            loadPlaylists();
            this.InitializeComponent();
        }

        private async void loadPlaylists()
        {
            List<Playlist> playlists = await _appController.DataManager.GetUserPlaylists();
            _playlists = new ObservableCollection<Playlist>(playlists);
            NotifyPropertyChanged("Playlists");
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
