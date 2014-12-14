using System;
using System.Threading.Tasks;

namespace SoundCloud.Interfaces
{
    public interface IApplicationController
    {
        Task<bool> IsUserLoggedIn();
        void SetStartupPage();

        void NavigateToPage(Type pageType);
    }
}
