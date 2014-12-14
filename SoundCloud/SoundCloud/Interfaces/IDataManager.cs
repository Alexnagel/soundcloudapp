using System.Threading.Tasks;
using SoundCloud.Services;
using SoundCloud.Services.Authentication;

namespace SoundCloud.Interfaces
{
    public interface IDataManager
    {
        void SetSoundCloudClient(SoundCloudClient client);
        Task<AccessToken> GetUserAccessToken();
        void SaveAccessToken();
    }
}
