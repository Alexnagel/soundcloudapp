using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using SoundCloud.Services;
using SoundCloud.Services.Enums;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace SoundCloud.Model
{
    [DataContract]
    public class ScStream
    {
        [PrimaryKey, AutoIncrement]
        [IgnoreDataMember]
        public int Id { get; set; }

        [IgnoreDataMember]
        public DateTime LastUpdated { get; set; }

        [DataMember(Name = "next_href")]
        public string NextHref { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        [DataMember(Name = "collection")]
        public List<CollectionItem> StreamItemList { get; set; }

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
