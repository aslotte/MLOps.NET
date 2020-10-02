using System;

namespace MLOps.NET.Exceptions
{
    /// <summary>
    /// Custom exception for failure to run kubectl create namespace
    /// </summary>
    public class CreateNamespaceException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        public CreateNamespaceException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public CreateNamespaceException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
