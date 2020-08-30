using System;

namespace MLOps.NET.Exceptions
{
    /// <summary>
    /// Custom exception for failure to run docker image rm
    /// </summary>
    public class DockerRemoveImageException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        public DockerRemoveImageException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public DockerRemoveImageException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
