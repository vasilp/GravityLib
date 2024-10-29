using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GravityLib.Common.Exceptions
{
    public static class ExceptionHelper
    {
        public static IList<string> TransientExceptionMessages = new List<string>()
        {
            "UNABLE_TO_LOCK_ROW, unable to obtain exclusive access to this record",
            "group membership operation already in progress",
            "No Content",
            "DELETE_FAILED, delete failed for this entity", //US65161 - Concurrency issue after integration with HR Gateway. Commands UpdateAccount and DeactivateAccount are processed at the same time creating a dealock on Salesforce. This is a temporary transient message chcek, SF should fix it at its end.
            "Could not create SSL/TLS secure channel"
        };

        public static bool IsTransient(HttpStatusCode httpStatusCode, string responseContent = null)
        {
            var isTransient = httpStatusCode == HttpStatusCode.GatewayTimeout ||
                              httpStatusCode == HttpStatusCode.RequestTimeout ||
                              httpStatusCode == HttpStatusCode.ServiceUnavailable;

            return isTransient || IsTransient(responseContent);
        }

        public static bool IsTransient(HttpStatusCode httpStatusCode, IDictionary<string, string[]> errors)
        {
            var isTransient = httpStatusCode == HttpStatusCode.GatewayTimeout ||
                              httpStatusCode == HttpStatusCode.RequestTimeout ||
                              httpStatusCode == HttpStatusCode.ServiceUnavailable;

            if (isTransient || errors == null)
                return isTransient;

            return errors.Any(e => e.Value.Any(IsTransient));
        }

        public static bool IsTransient(WebExceptionStatus exceptionStatus, string responseContent)
        {
            var isTransient = exceptionStatus == WebExceptionStatus.Timeout ||
                              exceptionStatus == WebExceptionStatus.SecureChannelFailure;

            return isTransient || IsTransient(responseContent);
        }

        public static bool IsTransient(string responseContent)
        {
            return !string.IsNullOrWhiteSpace(responseContent) &&
                   TransientExceptionMessages.Any(e => responseContent.ToUpper().Contains(e.ToUpper()));
        }
    }
}
