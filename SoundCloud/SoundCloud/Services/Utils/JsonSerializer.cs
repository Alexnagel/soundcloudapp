using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SoundCloud.Services.Utils
{
    public class JsonSerializer
    {
        /// <summary>
        /// Turn object in to a json format
        /// </summary>
        /// <typeparam name="T">The object ype</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            MemoryStream stream = new MemoryStream();

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            serializer.WriteObject(stream, obj);

            string result = Encoding.UTF8.GetString(stream.ToArray(), 0, stream.ToArray().Length);

            return result;
        }

        /// <summary>
        /// Deserialize an object from json
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="json">The json</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(json));

            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T));

            T result = (T)deserializer.ReadObject(stream);

            return result;
        }
    }
}
