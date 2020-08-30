using System;

namespace MLOps.NET.Exceptions
{
    /// <summary>
    /// Custom exception for failure to run docker build
    /// </summary>
    public class DockerBuildException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        public DockerBuildException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public DockerBuildException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
