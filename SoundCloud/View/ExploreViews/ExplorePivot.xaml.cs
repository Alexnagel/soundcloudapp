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
using SoundCloud.Services.Events;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SoundCloud.View.ExploreViews
{
    public sealed partial class ExplorePivot : UserControl, INotifyPropertyChanged
    {
        private AppController _appController;
        private DataManager _dataManager;
        private ObservableCollection<String> _exploreCategories;

        #region Data Binding
        public RelayCommand CategoryTap { get; private set; }
        #endregion Data Binding

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
            InitRelayCommand();
            this.InitializeComponent();
        }

        private async void InitCategories()
        {
            _exploreCategories = await _dataManager.GetExploreCategories();
            NotifyPropertyChanged("Categories");
        }
        
        private void InitRelayCommand()
        {
            CategoryTap = new RelayCommand(ShowTracksForCategory);
        }

        private void ShowTracksForCategory(object commandParam)
        {
            TappedRoutedEventArgs e = (TappedRoutedEventArgs)commandParam;
            if(e.OriginalSource.GetType() == typeof(TextBlock))
            {
                TextBlock source = (TextBlock)e.OriginalSource;
                String chosenCatgeory = source.Text;

                Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => _appController.NavigateToPage(typeof(ExplorePage), chosenCatgeory));
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
