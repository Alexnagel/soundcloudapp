using SoundCloud.Audio;
using SoundCloud.Controller;
using SoundCloud.Data;
using SoundCloud.Model.Explore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SoundCloud.View.ExploreViews
{
    public sealed partial class ExplorePivot : UserControl, INotifyPropertyChanged
    {
        #region Variables

        private ExploreLoadingCollection _exploreTracks;
        private AppController _appController;
        private DataManager _dataManager;
        private AudioManager _audioManager;

        private readonly object _padlock = new object();

        #endregion Variables

        #region Properties

        private ExploreLoadingCollection Categories
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
            DataContext = this;

            _appController = AppController.ControllerInstance;
            _dataManager = _appController.DataManager;

            _audioManager = _appController.AudioManager;
            _audioManager.UserAuth = _dataManager.GetUserAuthKey();

            InitExploreItems();

            //ClickCommand = new RelayCommand(ShowCategoryTracks);

            this.InitializeComponent();
        }

        public void InitExploreItems()
        {
            if (!_audioManager.IsPlaying)
                _audioManager.EmptyPlaylist();

            _exploreTracks = new ExploreLoadingCollection(_dataManager);
            _exploreTracks.CollectionChanged += _exploreTracks_CollectionChanged;
            NotifyPropertyChanged("Categories");
        }

        private void _exploreTracks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
