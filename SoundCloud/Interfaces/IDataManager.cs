using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SoundCloud.Model;
using SoundCloud.Services;
using SoundCloud.Services.Authentication;
using System;

namespace SoundCloud.Interfaces
{
    public interface IDataManager
    {
        void SetSoundCloudClient(SoundCloudClient client);
        Task<AccessToken> GetUserAccessToken();
        void SaveAccessToken();

        Task<ObservableCollection<CollectionItem>> GetStream();
        Task<ObservableCollection<CollectionItem>> GetNextPageStream(int lastId);

        Task<ObservableCollection<Track>> GetNextCatgoryTracks();

        Task<ObservableCollection<Track>> GetCategoryStream();

        Task<ObservableCollection<String>> GetExploreCategories();
    }
}
