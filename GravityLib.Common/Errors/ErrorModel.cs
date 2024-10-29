using System.Collections.Generic;

namespace GravityLib.Common.Errors
{
    /// <summary>
    /// Model to that provides a standard structure for error responses.
    /// This is compatible with the commonly used .Net WebAPI "ValidationProblemDetails" error model structure.
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Gets or sets the Http status code associated with this response.
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets any detailed error messages associated with the exception (e.g. validation errors)
        /// </summary>
        public IDictionary<string, string[]> Errors { get; set; }

        /// <summary>
        /// Gets or sets the trace identifier of the message. Use this to track logging and errors both on server side and client side using the same identifier.
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// Gets or sets the stack trace of the exception.
        /// </summary>
        public string StackTrace { get; set; }
    }
}
