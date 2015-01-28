using System;

namespace SoundCloud.Exceptions
{
    public class SoundCloudException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoundCloudException"/> class.
        /// </summary>
        public SoundCloudException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundCloudException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception message.</param>
        public SoundCloudException(string message) : base(message) { }
    }
}
