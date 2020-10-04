using System;

namespace MLOps.NET.Exceptions
{
    /// <summary>
    /// Custom exception for lack of registered model schema
    /// </summary>
    public class ModelSchemaNotRegisteredException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        public ModelSchemaNotRegisteredException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public ModelSchemaNotRegisteredException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
