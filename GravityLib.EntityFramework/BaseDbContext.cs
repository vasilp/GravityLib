using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GravityLib.Common;
using GravityLib.EntityFramework.Extensions;
using GravityLib.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GravityLib.EntityFramework
{
    public class BaseDbContext : DbContext
    {
        /// <summary>
        /// Contains the list of entities that should be physically deleted (not soft-deleted)
        /// </summary>
        protected List<object> PhysicalDeletes = new();

        public ISessionData SessionData { get; protected set; }

        /// <inheritdoc />
        public BaseDbContext(DbContextOptions options, ISessionData sessionData)
            : base(options)
        {
            SessionData = sessionData;
        }

        /// <summary>
        /// Optimizes the DbContext for read-only operations by switching off the AutoDetectChanges and Query tracking for the whole DbContext.
        /// To bring them back just call the method with parameter "false".
        /// </summary>
        public virtual BaseDbContext OptimizeForRead(bool switchOptimization = true)
        {
            ChangeTracker.QueryTrackingBehavior = switchOptimization ? QueryTrackingBehavior.NoTracking : QueryTrackingBehavior.TrackAll;
            ChangeTracker.AutoDetectChangesEnabled = !switchOptimization;

            // Enable chaining if needed anywhere
            return this;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyCustomDataAnnotations();

            modelBuilder.ApplyGlobalFilter<ISoftDeletableModel>(x => x.DeletedAt == null);
        }

        /// <summary>
        /// Restores already soft-deleted entity in the database
        /// </summary>
        public virtual void RestoreSoftDeleted<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity is ISoftDeletableModel softDeletable)
            {
                softDeletable.DeletedAt = null;
                softDeletable.DeletedBy = null;
            }
        }

        /// <summary>
        /// Marks the entity to be physically deleted from the database (not soft-delete)
        /// </summary>
        public virtual void PhysicalDelete<TEntity>(TEntity entity) where TEntity : class
        {
            Remove(entity);

            PhysicalDeletes.Add(entity);
        }

        /// <summary>
        /// Marks the entities to be physically deleted from the database (not soft-delete)
        /// </summary>
        public virtual void PhysicalDeleteRange<TEntity>(ICollection<TEntity> entity) where TEntity : class
        {
            RemoveRange(entity);

            PhysicalDeletes.AddRange(entity);
        }

        /// <inheritdoc />
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ProcessEntitiesBeforeSave();

            var result = base.SaveChanges(acceptAllChangesOnSuccess);

            PhysicalDeletes.Clear();
            return result;
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            ProcessEntitiesBeforeSave();

            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            PhysicalDeletes.Clear();
            return result;
        }

        #region Auditable & Soft-Delete entitites auto-update

        protected virtual void ProcessEntitiesBeforeSave()
        {
            var utcNow = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        BeforeSavePendingAddedEntity(entry, utcNow);
                        break;

                    case EntityState.Modified:
                        BeforeSavePendingModifiedEntity(entry, utcNow);
                        break;

                    case EntityState.Deleted:
                        BeforeSavePendingDeletedEntity(entry, utcNow);
                        break;
                }
            }
        }

        protected virtual void BeforeSavePendingAddedEntity(EntityEntry entry, DateTime utcNow)
        {
            if (!(entry.Entity is IAuditableModel auditable))
                return;

            auditable.CreatedAt = utcNow;
            auditable.UpdatedAt = utcNow;

            // User auditable
            if (entry.Entity is IUserAuditableModel userAuditable && SessionData?.UserId != null)
            {
                // do not set the value if it is already the same, so EF will not pass it in the update SQL statement
                if (userAuditable.CreatedBy != SessionData.UserId)
                {
                    userAuditable.CreatedBy = SessionData.UserId;
                }

                // do not set the value if it is already the same, so EF will not pass it in the update SQL statement
                if (userAuditable.UpdatedBy != SessionData.UserId)
                {
                    userAuditable.UpdatedBy = SessionData.UserId;
                }
            }
        }

        protected virtual void BeforeSavePendingModifiedEntity(EntityEntry entry, DateTime utcNow)
        {
            if (!(entry.Entity is IAuditableModel auditable))
                return;

            auditable.UpdatedAt = utcNow;

            // User auditable
            if (entry.Entity is IUserAuditableModel userAuditable && SessionData?.UserId != null)
            {
                // do not set the value if it is already the same, so EF will not pass it in the update SQL statement
                if (userAuditable.UpdatedBy != SessionData.UserId)
                {
                    userAuditable.UpdatedBy = SessionData.UserId;
                }
            }
        }

        protected virtual void BeforeSavePendingDeletedEntity(EntityEntry entry, DateTime utcNow)
        {
            // Skip setting deleted flags - we want to physically delete the entity
            if (PhysicalDeletes.Contains(entry.Entity))
                return;

            if (entry.Entity is ISoftDeletableModel deletable)
            {
                deletable.DeletedAt = utcNow;
                deletable.DeletedBy = SessionData?.UserId;

                // Change from Deleted to Modified as it is soft deletable
                entry.State = EntityState.Modified;
            }
        }

        #endregion
    }
}
