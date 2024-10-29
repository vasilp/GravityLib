using System.Collections.Generic;

namespace GravityLib.Common
{
    /// <summary>
    /// Intended to store the user information for the current execution/session of an action.
    /// In Web API this could be obtained from the authentication mechanisms for the current request.
    /// For the services / console / desktop apps, this could be obtained from the current thread's information for the user/system that is performing the specific action(s).
    /// </summary>
    public interface ISessionData
    {
        /// <summary>
        /// This is the internal ID of the user in the current application's database.
        /// Typically it used for automated user filtering, access and basic audits.
        /// </summary>
        long? UserId { get; set; }

        string SSOId { get; }

        string Name { get; }

        string Username { get; }

        string Email { get; }

        string Roles { get; }

        IEnumerable<string> RolesEnumerable { get; }
    }
}
