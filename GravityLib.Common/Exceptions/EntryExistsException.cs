using System;
using System.Runtime.Serialization;

namespace GravityLib.Common.Exceptions
{
    /// <summary>
    /// Thrown when a user attempts to create entry that is already existing in the system.
    /// </summary>
    /// <remarks>
    /// An API would typically handle this error by returning an HTTP 409 error.
    /// </remarks>
    [Serializable]
    public class EntryExistsException : Exception
    {
        /// <inheritDoc/>
        public EntryExistsException()
        {
        }

        /// <inheritDoc/>
        public EntryExistsException(string message)
            : base(message)
        {
        }

        /// <inheritDoc/>
        public EntryExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritDoc/>
        protected EntryExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
