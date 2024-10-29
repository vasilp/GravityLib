using System;
using System.Runtime.Serialization;

namespace GravityLib.Common.Exceptions
{
    /// <summary>
    /// Thrown when a user attempts to access data that they have not been authorized to use.
    /// </summary>
    /// <remarks>
    /// An API would typically handle this error by returning an HTTP 403 error.
    /// </remarks>
    [Serializable]
    public class AccessDeniedException : Exception
    {
        /// <inheritDoc/>
        public AccessDeniedException()
        {
        }

        /// <inheritDoc/>
        public AccessDeniedException(string message)
            : base(message)
        {
        }

        /// <inheritDoc/>
        public AccessDeniedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritDoc/>
        protected AccessDeniedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
