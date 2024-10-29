using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GravityLib.Common.Extensions;

namespace GravityLib.Common.Exceptions
{
    /// <summary>
    /// Thrown when a user attempts to access data that they have not been authorized to use.
    /// </summary>
    /// <remarks>
    /// An API would typically handle this error by returning an HTTP 400 error.
    /// </remarks>
    [Serializable]
    public class ValidationException : Exception, IExceptionWithErrors
    {
        private const string ValidationFailed = "Validation Failed";

        /// <inheritDoc/>
        public IDictionary<string, string[]> Errors { get; private set; }

        /// <summary>
        /// Construct a <see cref="ValidationException"/> with given parameters
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception</param>
        /// <param name="errors">List of errors</param>
        public ValidationException(string message = ValidationFailed, IDictionary<string, string[]> errors = null, Exception inner = null)
            : base(message, inner)
        {
            Errors = errors;
        }

        public ValidationException(string propertyKey, string errorMessage)
            : this()
        {
            Errors = new Dictionary<string, string[]> { { propertyKey, new [] { errorMessage } } };
        }

        public ValidationException(string propertyKey, string[] errorMessages)
            : this()
        {
            Errors = new Dictionary<string, string[]> { { propertyKey, errorMessages } };
        }

        /// <inheritDoc/>
        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritDoc/>
        public string MessageWithErrors()
        {
            return Errors != null && Errors.Any()
                ? $"{Message}{Environment.NewLine}Errors: {Errors.ToJson()}"
                : Message;
        }

        /// <inheritDoc/>
        public override string ToString()
        {
            return MessageWithErrors() + Environment.NewLine + base.ToString();
        }
    }
}
