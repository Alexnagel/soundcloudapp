using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SoundCloud.Services.Utils
{
    public static class Util
    {
        #region Licenses

        public const string NoRightsReserved = "no-rights-reserved";
        public const string AllRightsReserved = "all-rights-reserved";
        public const string CcBy = "cc-by";
        public const string CcByNc = "cc-by-nc";
        public const string CcByNd = "cc-by-nd";
        public const string CcBySa = "cc-by-sa";
        public const string CcByNcNd = "cc-by-nc-nd";
        public const string CcByNcSa = "cc-by-nc-sa";

        #endregion

        #region Uri

        /// <summary>
        /// Returns an Uri after replacement of the format item with the corresponding string representation.
        /// </summary>
        /// 
        /// <param name="uri">Input Uri.</param>
        /// <param name="keys">Format items.</param>
        public static Uri With(this Uri uri, params object[] keys)
        {
            return new Uri(string.Format(uri.ToString(), keys));
        }

        /// <summary>
        /// Returns a Uri with authorization segment.
        /// </summary>
        /// 
        /// <param name="baseUri">Input Uri.</param>
        /// <param name="token">Token.</param>
        public static Uri UriWithAuthorizedUri(this Uri baseUri, string token)
        {
            return baseUri.UriAppendingQueryString("oauth_token", token);
        }

        /// <summary>
        /// Returns a Uri with authorization segment.
        /// </summary>
        /// <param name="baseUri">Input Uri.</param>
        /// <param name="clientID">The client ID.</param>
        /// <returns></returns>
        public static Uri UriWithClientID(this Uri baseUri, string clientID)
        {
            return baseUri.UriAppendingQueryString("client_id", clientID);
        }

        /// <summary>
        /// Adds query strings to a given uri.
        /// </summary>
        /// 
        /// <param name="baseUri">Input uri.</param>
        /// <param name="parameters">Dictionnary of^parameters to add.</param>
        public static Uri UriAppendingParameters(this Uri baseUri, Dictionary<string, object> parameters)
        {

            var sb = new StringBuilder();

            foreach (KeyValuePair<string, object> pair in parameters)
            {
                sb.AppendFormat("{0}={1}&", pair.Key, pair.Value);
            }

            return baseUri.UriAppendingQueryString(sb.ToString().TrimEnd('&'));
        }
        public static Uri UriAppendingQueryString(this Uri uri, string name, string value)
        {
            return
                new UriBuilder(uri)
                {
                    Query = (uri.Query + "&" + name + "=" + value).TrimStart('&').TrimStart('?')
                }
                    .Uri;
        }
        public static Uri UriAppendingQueryString(this Uri uri, string querystring)
        {
            return
                new UriBuilder(uri)
                {
                    Query = (uri.Query + "&" + querystring).TrimStart('&')
                }
                .Uri;
        }

        public static Dictionary<string, string> UriToFormData(Uri uri)
        {
            string urlData = WebUtility.UrlDecode(uri.ToString()).Split('?')[1];
            string[] parameters = urlData.Split('&');

            var formData = new Dictionary<string, string>();
            for (int i = 0; i < parameters.Length; i++)
            {
                string[] param = parameters[i].Split('=');
                formData.Add(param[0], param[1]);
            }
            return formData;
        }

        #endregion Uri
    }
}
