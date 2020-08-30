using System;

namespace MLOps.NET.Exceptions
{
    /// <summary>
    /// Custom exception for failure to install dotnet new templates
    /// </summary>
    public class TemplateInstallationException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        public TemplateInstallationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public TemplateInstallationException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
