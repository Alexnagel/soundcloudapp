// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BackgroundAudio.PlayQueue;
using SoundCloud.Controller;
using SoundCloud.Model;

namespace SoundCloud.View.UserViews
{
    public sealed partial class MePivot : UserControl, INotifyPropertyChanged
    {
        #region Databinding
        public static DependencyProperty UserIdProperty = DependencyProperty.Register("UserId",
            typeof(int), typeof(TrackListItem), new PropertyMetadata(0));

        public int UserId
        {
            get { return (int)this.GetValue(UserIdProperty); }
            set
            {
                this.SetValue(UserIdProperty, value);
            }
        }

        public User User
        {
            get {  return _user; }
            set
            {
                if (value != _user)
                {
                    _user = value;
                    NotifyPropertyChanged("User");
                }
            }
        }

        public string MeFollowers
        {
            get { return _user.Followers + " followers"; }
        }

        public string MeFollowing
        {
            get { return _user.Followings + " following"; }
        }

        public ObservableCollection<Track> MeTracks
        {
            get { return _meTracks; }
            set
            {
                if (value != _meTracks)
                {
                    _meTracks = value;
                    NotifyPropertyChanged("MeTracks");
                }
            }
        }

        public ObservableCollection<User> MeFollowersUsers
        {
            get { return _meFollowersUsers; }
            set
            {
                if (value != _meFollowersUsers)
                {
                    _meFollowersUsers = value;
                    NotifyPropertyChanged("MeFollowersUsers");
                }
            }
        }

        public ObservableCollection<User> MeFollowingUsers
        {
            get { return _meFollowingUsers; }
            set
            {
                if (value != _meFollowingUsers)
                {
                    _meFollowingUsers = value;
                    NotifyPropertyChanged("MeFollowingUsers");
                }
            }
        }
        #endregion Databinding

        #region variables

        private User _user;
        private AppController _appController;

        private ObservableCollection<Track> _meTracks;
        private ObservableCollection<User> _meFollowersUsers;
        private ObservableCollection<User> _meFollowingUsers; 
        #endregion variables

        public MePivot()
        {
            _appController = AppController.ControllerInstance;

            if (UserId == 0)
            {
                _user = _appController.DataManager.CurrentUser;
                loadPivots();
            }

            PropertyChanged += MePivot_PropertyChanged;

            this.InitializeComponent();
        }

        public MePivot(int userId)
        {
            _appController = AppController.ControllerInstance;

            if (userId != 0)
            {
                UserId = userId;
                loadUser();
            }

            PropertyChanged += MePivot_PropertyChanged;
        }

        void MePivot_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UserId")
                loadUser();
        }

        private async void loadUser()
        {
            _user = await User.GetUser(UserId);
            loadPivots();
            
        }

        private void loadPivots()
        {
            loadTracks();
            loadFollowers();
            loadFollowing();

            this.InitializeComponent();
        }

        private async void loadTracks()
        {
            var tracks = await _user.GetTracks();
            _meTracks = new ObservableCollection<Track>(tracks);

            var _appController = AppController.ControllerInstance;
            if (!_appController.AudioManager.IsPlaying)
                _appController.AudioManager.EmptyPlaylist(QueueType.User);

            _appController.AudioManager.AddToPlaylist(tracks.ToList(), QueueType.User);

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => NotifyPropertyChanged("MeTracks"));
        }

        private async void loadFollowers()
        {
            var followers = await _user.GetFollowers();
            _meFollowersUsers = new ObservableCollection<User>(followers);

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => NotifyPropertyChanged("MeFollowersUsers"));
        }

        private async void loadFollowing()
        {
            var following = await _user.GetFollowings();
            _meFollowingUsers = new ObservableCollection<User>(following);

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => NotifyPropertyChanged("MeFollowingUsers"));
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
