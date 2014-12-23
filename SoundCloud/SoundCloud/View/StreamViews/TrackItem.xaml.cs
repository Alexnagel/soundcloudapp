using Windows.ApplicationModel.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using SoundCloud.Audio;
using SoundCloud.Model;
using SoundCloud.Services.Events;

namespace SoundCloud.View
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

        public TrackItem()
        {
            initRelayCommands();
            this.InitializeComponent();
        }

        private void PlaySong(object commandParam)
        {

            AudioManager manager = AudioManager.ManagerInstance;
            manager.PlayTrack(CollectionItem.Id);
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof (NowPlayingPage));
        }

        private void initRelayCommands()
        {
            TapCommand = new RelayCommand(PlaySong);
        }
    }
}
