using System;

namespace SoundCloud.Services.Events
{
    public class SoundCloudEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Json response returned by the server.
        /// </summary>
        public string RawResponse { get; set; }

        /// <summary>
        /// Type of returned type.
        /// </summary>
        public Type ReturnedType { get; set; }

        #endregion Properties
    }
}
