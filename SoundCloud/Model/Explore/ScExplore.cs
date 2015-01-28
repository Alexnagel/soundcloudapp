using SoundCloud.Services;
using SoundCloud.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SoundCloud.Model.Explore
{
    [DataContract]
    class ScExplore
    {
        [DataMember(Name = "next_href")]
        public string NextHref { get; set; }

        [DataMember(Name = "music")]
        public List<String> MusicCategories { get; set; }

        [DataMember(Name = "audio")]
        public List<String> AudioCategories { get; set; }

        public static async Task<ScExplore> GetMusicExploreCategories()
        {
            return await SoundCloudWrapper.ApiAction<ScExplore>(ApiCall.Explore);
        }

        public static async Task<ScExplore> GetAudioExploreCategories()
        {
            return await SoundCloudWrapper.ApiAction<ScExplore>(ApiCall.Explore);
        }
    }
}
