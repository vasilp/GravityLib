namespace GravityLib.EntityFramework.Interfaces
{
    /// <summary>
    /// Adds audit information to a model entity
    /// </summary>
    public interface IUserAuditableModel : IAuditableModel
    {
        /// <summary>
        /// The ID of the user who created the record.
        /// </summary>
        long? CreatedBy { get; set; }

        /// <summary>
        /// The ID of the the user who last updated the record.
        /// </summary>
        long? UpdatedBy { get; set; }
    }
}
