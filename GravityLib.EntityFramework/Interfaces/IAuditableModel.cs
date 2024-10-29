using System;

namespace GravityLib.EntityFramework.Interfaces
{
    public interface IAuditableModel
    {
        /// <summary>
        /// The UTC date and time on which the entity was created.
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// The UTC date and time on which the entity was last updated.
        /// </summary>
        DateTime UpdatedAt { get; set; }
    }
}
