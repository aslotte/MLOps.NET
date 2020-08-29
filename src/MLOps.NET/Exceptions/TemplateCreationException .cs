using System;

namespace MLOps.NET.Exceptions
{
    /// <summary>
    /// Custom exception for failure to run dotnet new template
    /// </summary>
    public class TemplateCreationException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        public TemplateCreationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public TemplateCreationException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
