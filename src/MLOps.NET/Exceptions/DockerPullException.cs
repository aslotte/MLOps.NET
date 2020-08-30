using System;

namespace MLOps.NET.Exceptions
{
    /// <summary>
    /// Custom exception for failure to run docker pull
    /// </summary>
    public class DockerPullException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        public DockerPullException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public DockerPullException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
