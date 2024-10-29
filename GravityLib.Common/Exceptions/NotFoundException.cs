using System;
using System.Runtime.Serialization;

namespace GravityLib.Common.Exceptions
{
    /// <summary>
    /// Thrown when a user attempts to access data that they have not been authorized to use.
    /// </summary>
    /// <remarks>
    /// An API would typically handle this error by returning an HTTP 404 error.
    /// </remarks>
    [Serializable]
    public class NotFoundException : Exception
    {
        /// <inheritDoc/>
        public NotFoundException()
        {
        }

        /// <inheritDoc/>
        public NotFoundException(string message)
            : base(message)
        {
        }

        /// <inheritDoc/>
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritDoc/>
        protected NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
