using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SoundCloud.Controller;
using SoundCloud.Model;
using SoundCloud.Services.Events;

namespace SoundCloud.View
{
    public sealed partial class TrackListItem : UserControl
    {
        #region Data Binding
        public RelayCommand TapCommand { get; private set; }

        public static readonly DependencyProperty PlaylistIdProperty = DependencyProperty.Register("PlaylistId",
            typeof(int), typeof(TrackListItem), new PropertyMetadata(-1));
        public static readonly DependencyProperty CollectionIdProperty = DependencyProperty.Register("CollectionId",
            typeof(string), typeof(TrackListItem), new PropertyMetadata(string.Empty));
        #endregion Data Binding

        public int PlaylistId { get; set; }
        public string CollectionId { get; set; }

        private Track _trackItem;
        private Track TrackItem
        {
            get
            {
                if (_trackItem == null)
                    _trackItem = DataContext as Track;
                return _trackItem;
            }
        }

        private readonly AppController _appController;

        public TrackListItem()
        {
            initRelayCommands();
            _appController = AppController.ControllerInstance;

            this.InitializeComponent();
        }

        private void PlaySong(object commandParam)
        {
            _appController.AudioManager.TrackLoaded += TrackLoadedHandler;

            string trackId = TrackItem.Id.ToString();
            if (PlaylistId != -1)
                trackId += PlaylistId.ToString();
            if (!String.IsNullOrEmpty(CollectionId))
                trackId += CollectionId;

            _appController.AudioManager.PlayTrack(trackId);
        }

        private void TrackLoadedHandler(string id)
        {
            string trackId = TrackItem.Id.ToString();
            if (PlaylistId != -1)
                trackId += PlaylistId.ToString();

            if (trackId == id)
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
