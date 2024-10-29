using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GravityLib.Common.Extensions;
using GravityLib.EntityFramework.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace GravityLib.EntityFramework.Extensions
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Global query filters are LINQ query predicates applied to Entity Types in the metadata model (usually in OnModelCreating).
        /// https://docs.microsoft.com/en-us/ef/core/querying/filters
        /// Filters can be ignored with ".IgnoreQueryFilters()" query method extension.
        /// </summary>
        public static void ApplyGlobalFilter<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> expression)
        {
            var entityTypes = modelBuilder.Model
                .GetEntityTypes()
                .Where(e => e.ClrType != null && e.ClrType.IsImplementationOf<TInterface>())
                .Select(e => e.ClrType);

            foreach (var entityType in entityTypes)
            {
                modelBuilder.ApplyGlobalFilter<TInterface>(entityType, expression);
            }
        }

        /// <summary>
        /// Global query filters are LINQ query predicates applied to Entity Types in the metadata model (usually in OnModelCreating).
        /// https://docs.microsoft.com/en-us/ef/core/querying/filters
        /// Filters can be ignored with ".IgnoreQueryFilters()" query method extension.
        /// </summary>
        public static void ApplyGlobalFilter<TInterface>(this ModelBuilder modelBuilder, Type entityType, Expression<Func<TInterface, bool>> expression)
        {
            var newParam = Expression.Parameter(entityType);
            var newbody = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
            modelBuilder.Entity(entityType).HasQueryFilter(Expression.Lambda(newbody, newParam));
        }

        /// <summary>
        /// Apply custom data annotations entity configurations
        /// </summary>
        public static void ApplyCustomDataAnnotations(this ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model
                .GetEntityTypes()
                .Where(e => e.ClrType != null);

            foreach (var entityType in entityTypes)
            {
                // Apply class attributes
                var typeAttr = entityType.ClrType.GetCustomAttributes<BaseEFModelAttribute>();
                foreach (var attribute in typeAttr)
                {
                    attribute.ApplyToModelBuilder(entityType.ClrType, modelBuilder);
                }

                // Apply property attributes
                foreach (var property in entityType.GetProperties())
                {
                    if (property.PropertyInfo == null)
                        continue;

                    var propAttr = property.PropertyInfo.GetCustomAttributes<BaseEFModelPropertyAttribute>();
                    foreach (var attribute in propAttr)
                    {
                        attribute.ApplyToModelBuilder(entityType.ClrType, property.PropertyInfo, modelBuilder);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the cascade delete behavior from all foreign keys set up in the DbContext up to now.
        /// This method sets the DeleteBehavior = DeleteBehavior.Restrict, which is effectively
        /// NoAction, set to null if possible (nullable type) or throw exception (SQL NoAction).
        /// </summary>
        public static void RemoveOneToManyCascade(this ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                // Set to null if possible (nullable type) or throw exception (SQL NoAction)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
