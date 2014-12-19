using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using SoundCloud.Services;
using SoundCloud.Services.Enums;

namespace SoundCloud.Model
{
    [DataContract]
    public class ScStream
    {
        [DataMember(Name = "next_href")]
        public string NextHref { get; set; }

        [DataMember(Name = "collection")]
        public ObservableCollection<CollectionItem> StreamItems { get; set; }

        public static async Task<ScStream> GetStream()
        {
            return await SoundCloudWrapper.ApiAction<ScStream>(ApiCall.MeStream);
        }

        public static async Task<ScStream> GetNextStream(string nextHref)
        {
            Uri nextUri = new Uri(nextHref);
            return await SoundCloudWrapper.ApiAction<ScStream>(nextUri);
        }
    }
}
