using System;

namespace MLOps.NET.Exceptions
{
    /// <summary>
    /// Custom exception for failure to run kubectl apply
    /// </summary>
    public class KubectlApplyException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        public KubectlApplyException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public KubectlApplyException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
