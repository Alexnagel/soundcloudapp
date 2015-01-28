using System.Collections.Specialized;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using SoundCloud.Audio;
using SoundCloud.Controller;
using SoundCloud.Model.Explore;
using System.Collections.ObjectModel;
using System;
using SoundCloud.Data;
using System.Collections.Generic;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SoundCloud.View.ExploreViews
{
    public sealed partial class ExplorePivot : UserControl, INotifyPropertyChanged
    {
        private AppController _appController;
        private DataManager _dataManager;
        private ObservableCollection<String> _exploreCategories;

        public ObservableCollection<String> Categories
        {
            get { return _exploreCategories; }
            set
            {
                if (_exploreCategories != value)
                {
                    _exploreCategories = value;
                    NotifyPropertyChanged("Categories");
                }
            }
        }
        public ExplorePivot()
        {
            _appController = AppController.ControllerInstance;
            _dataManager = _appController.DataManager;
            InitCategories();
        }

        private async void InitCategories()
        {
            _exploreCategories = await _dataManager.GetExploreCategories();
            //_exploreCategories = new ObservableCollection<String>();
           // _exploreCategories.Add("hallo");
            NotifyPropertyChanged("Categories");
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
