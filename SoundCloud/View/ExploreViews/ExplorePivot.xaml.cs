using System.Collections.Specialized;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using SoundCloud.Audio;
using SoundCloud.Controller;
using SoundCloud.Model.Explore;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SoundCloud.View.ExploreViews
{
    public sealed partial class ExplorePivot : UserControl, INotifyPropertyChanged
    {
        #region Variables

        private ExploreLoadingCollection _exploreTracks;
        private AppController _appController;
        private AudioManager _audioManager;

        private readonly object _padlock = new object();

        #endregion Variables

        #region Properties

        public ExploreLoadingCollection Categories
        {
            get { return _exploreTracks; }
            set
            {
                if (_exploreTracks != value)
                {
                    _exploreTracks = value;
                    NotifyPropertyChanged("Categories");
                }
            }
        }

        #endregion Properties

        public ExplorePivot()
        {
            _appController = AppController.ControllerInstance;
            _audioManager = _appController.AudioManager;

            InitExploreItems();

            //ClickCommand = new RelayCommand(ShowCategoryTracks);

            this.InitializeComponent();
        }

        public void InitExploreItems()
        {
            if (!_audioManager.IsPlaying)
                _audioManager.EmptyPlaylist();

            _exploreTracks = new ExploreLoadingCollection(_appController.DataManager);
            _exploreTracks.CollectionChanged += _exploreTracks_CollectionChanged;
            NotifyPropertyChanged("Categories");
        }

        private void _exploreTracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (_padlock)
            {
                //Todo audiomanager shit
                //_audioManager.AddCatTrack(_exploreTracks[_exploreTracks.Count - 1]);
            }
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

        #endregion INotifyPropertyChanged Members
    }
}
