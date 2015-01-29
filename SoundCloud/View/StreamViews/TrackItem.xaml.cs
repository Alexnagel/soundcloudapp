using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using BackgroundAudio.PlayQueue;
using SoundCloud.Audio;
using SoundCloud.Controller;
using SoundCloud.Model;
using SoundCloud.Services.Events;

namespace SoundCloud.View.StreamViews
{
    public sealed partial class TrackItem : UserControl
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

        public TrackItem()
        {
            initRelayCommands();
            _appController = AppController.ControllerInstance;

            this.InitializeComponent();
        }

        private void PlaySong(object commandParam)
        {
            _appController.AudioManager.TrackLoaded += TrackLoadedHandler;

            _appController.AudioManager.PlayTrack(CollectionItem.Id, QueueType.Stream);
        }

        private void TrackLoadedHandler(string id)
        {
            if (_collectionItem.Id.Equals(id))
            {
                _appController.AudioManager.TrackLoaded -= TrackLoadedHandler;

                // go to the now playing page
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => _appController.NavigateToPage(typeof (NowPlayingPage)));
            }
        }

        private void initRelayCommands()
        {
            TapCommand = new RelayCommand(PlaySong);
        }
    }
}
