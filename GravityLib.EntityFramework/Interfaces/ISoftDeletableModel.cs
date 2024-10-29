using System;

namespace GravityLib.EntityFramework.Interfaces
{
    /// <summary>
    /// Adds soft deletable feature to the model entity.
    /// </summary>
    public interface ISoftDeletableModel
    {
        /// <summary>
        /// The UTC date and time on which the record was deleted.
        /// </summary>
        DateTime? DeletedAt { get; set; }

        /// <summary>
        /// The external identifier of the the user who has deleted the record.
        /// </summary>
        long? DeletedBy { get; set; }
    }
}
