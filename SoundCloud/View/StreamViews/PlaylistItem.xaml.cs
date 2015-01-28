using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using SoundCloud.Controller;
using SoundCloud.Model;
using SoundCloud.Services.Events;

namespace SoundCloud.View.StreamViews
{
    public sealed partial class PlaylistItem : UserControl
    {
        #region Data Binding
        public RelayCommand TapCommand { get; private set; }
        #endregion Data Binding

        private CollectionItem _collectionItem;
        private CollectionItem CollectionItem
        {
            get
            {
                if (_collectionItem == null)
                    _collectionItem = DataContext as CollectionItem;
                return _collectionItem;
            }
        }

        private readonly AppController _appController;

        public PlaylistItem()
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
