using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;

namespace GravityLib.Common.Exceptions
{
    /// <summary>
    /// Exception thrown in response to a failed call to an external API.
    /// </summary>
    [Serializable]
    public class RemoteException : Exception, IExceptionWithErrors
    {
        /// <summary> Gets or sets the status code of the failed request associated with the exception. </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary> Indicates whether or not the error is transient. </summary>
        public bool IsTransient { get; protected set; }

        /// <summary> Optionally store the response information returned </summary>
        public HttpResponseMessage Response { get; set; }

        /// <inheritDoc/>
        public IDictionary<string, string[]> Errors { get; private set; }


        /// <inheritDoc/>
        public RemoteException()
        {
        }

        /// <inheritDoc/>
        public RemoteException(string message)
            : base(message)
        {
        }

        /// <inheritDoc/>
        public RemoteException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteException"/> class.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        /// <param name="inner">The inner exception details for the exception. </param>
        /// <param name="isTransient">Indicates whether or not the exception is transient.</param>
        public RemoteException(string message, Exception inner, bool isTransient)
            : base(message, inner)
        {
            IsTransient = isTransient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteException"/> class.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        /// <param name="statusCode">The Http status code associated with the exception.</param>
        public RemoteException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
            IsTransient = ExceptionHelper.IsTransient(StatusCode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteException"/> class.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        /// <param name="statusCode">The Http status code associated with the exception.</param>
        /// <param name="isTransient">Indicates whether or not the exception is transient.</param>
        public RemoteException(string message, HttpStatusCode statusCode, bool isTransient)
            : base(message)
        {
            StatusCode = statusCode;
            IsTransient = isTransient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteException"/> class.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        /// <param name="statusCode">The Http status code associated with the exception.</param>
        /// <param name="errors">A collection of errors associated with the exception. </param>
        public RemoteException(string message, HttpStatusCode statusCode, IDictionary<string, string[]> errors)
            : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
            IsTransient = ExceptionHelper.IsTransient(StatusCode, Errors);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteException"/> class.
        /// This exception has been raised after getting a response that hasn't succeeded
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        /// <param name="response">The response that raised the webClient exception</param>
        public RemoteException(string message, HttpResponseMessage response)
            : base(message)
        {
            Response = response;
            StatusCode = response.StatusCode;
            IsTransient = ExceptionHelper.IsTransient(StatusCode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteException"/> class.
        /// This exception has been raised after getting a response that hasn't succeeded
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        /// <param name="response">The response that raised the webClient exception</param>
        /// <param name="isTransient">Indicate whether the exception is transient</param>
        public RemoteException(string message, HttpResponseMessage response, bool isTransient)
            : this(message, response.StatusCode, isTransient)
        {
            Response = response;
        }

        /// <inheritDoc/>
        protected RemoteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the flattened list of errors returned by the response.
        /// </summary>
        public string GetErrorsFlattened()
        {
            if (Errors == null || !Errors.Any())
                return string.Empty;

            // Denormalize the keys with their multiple values
            var details = Errors.SelectMany(x => x.Key.Select(k => $"{k}: {x.Value}"));

            return string.Join("," + Environment.NewLine, details);
        }
    }
}
