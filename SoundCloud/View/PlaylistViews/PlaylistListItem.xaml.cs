using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using SoundCloud.Controller;
using SoundCloud.Model;
using SoundCloud.Services.Events;

namespace SoundCloud.View
{
    public sealed partial class PlaylistListItem : UserControl
    {
        #region Data Binding
        public RelayCommand TapCommand { get; private set; }
        #endregion Data Binding

        private Playlist _playlist;
        private Playlist CollectionItem
        {
            get
            {
                if (_playlist == null)
                    _playlist = DataContext as Playlist;
                return _playlist;
            }
        }

        private readonly AppController _appController;

        public PlaylistListItem()
        {
            _appController = AppController.ControllerInstance;
            initRelayCommands();

            this.InitializeComponent();
        }

        private void NavigateToPlaylist(object commandParam)
        {
            // go to the playlist page
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => _appController.NavigateToPage(typeof(PlaylistPage), CollectionItem));
        }

        private void initRelayCommands()
        {
            TapCommand = new RelayCommand(NavigateToPlaylist);
        }
    }
}
