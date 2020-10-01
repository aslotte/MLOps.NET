using System;

namespace MLOps.NET.Exceptions
{
    /// <summary>
    /// Custom exception for failure to run kubectl create secret
    /// </summary>
    public class CreateImagePullSecretException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        public CreateImagePullSecretException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public CreateImagePullSecretException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
