using System.Collections.Generic;

namespace GravityLib.Common.Exceptions
{
    public interface IExceptionWithErrors
    {
        /// <summary>
        /// A dictionary of all members that have associated list of error messages related to the exception.
        /// </summary>
        IDictionary<string, string[]> Errors { get; }
    }
}
