using Windows.UI.Xaml.Controls;
using SoundCloud.Controller;
using SoundCloud.Services.Events;

namespace SoundCloud.View.UserViews
{
    public sealed partial class UserView : UserControl
    {
        public RelayCommand TapCommand { get; private set; }

        public UserView()
        {
            initRelayCommands();
            this.InitializeComponent();
        }

        private void ShowUser(object commandParam)
        {
            AppController.ControllerInstance.NavigateToPage(typeof(UserPage), (int)commandParam);
        }

        private void initRelayCommands()
        {
            TapCommand = new RelayCommand(ShowUser);
        }
    }
}
