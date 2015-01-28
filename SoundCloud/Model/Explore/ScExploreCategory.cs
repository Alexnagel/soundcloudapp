using SoundCloud.Services;
using SoundCloud.Services.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SoundCloud.Model.Explore
{
    [DataContract]
    class ScExploreCategory
    {
        [DataMember(Name = "next_href")]
        public string NextHref { get; set; }

        [DataMember(Name = "tracks")]
        public ObservableCollection<Track> CategoryTracks { get; set; }


        public static async Task<ScExploreCategory> GetCategoryTracks(String chosenCategory)
        {
            return await SoundCloudWrapper.ApiAction<ScExploreCategory>(ApiCall.ExploreCategories, chosenCategory);
        }

        public static async Task<ScExploreCategory> GetNextCatogoryTracks(string nextHref)
        {
            Uri nextUri = new Uri(nextHref);
            return await SoundCloudWrapper.ApiAction<ScExploreCategory>(nextUri);
        }
    }
}
