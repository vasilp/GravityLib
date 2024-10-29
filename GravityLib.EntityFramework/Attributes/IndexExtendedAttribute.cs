using System;
using Microsoft.EntityFrameworkCore;

namespace GravityLib.EntityFramework.Attributes
{
    /// <summary>
    /// Configures an index on the specified properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class IndexExtendedAttribute : BaseEFModelAttribute
    {
        /// <summary> The names of the properties that make up the index. </summary>
        public string[] PropertyNames { get; set; }

        /// <summary> Configures whether this index is unique (i.e. the value(s) for each instance must be unique). </summary>
        public bool IsUnique { get; set; } = false;

        /// <summary> Configures the name of the index in the database when targeting a relational database. </summary>
        /// <remarks> See <see href="https://aka.ms/efcore-docs-indexes">Indexes</see> for more information. </remarks>
        public string Name { get; set; } = null;

        /// <summary> Configures the filter expression for the index. </summary>
        /// <remarks> See <see href="https://aka.ms/efcore-docs-indexes">Indexes</see> for more information. </remarks>
        public string Filter { get; set; } = null;

        public IndexExtendedAttribute(params string[] propertyNames)
        {
            PropertyNames = propertyNames;
        }

        public override void ApplyToModelBuilder(Type t, ModelBuilder builder)
        {
            var indexBuilder = builder.Entity(t)
                .HasIndex(PropertyNames)
                .IsUnique(IsUnique);

            if (!string.IsNullOrWhiteSpace(Name))
            {
                indexBuilder.HasDatabaseName(Name);
            }

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                indexBuilder.HasFilter(Filter);
            }
        }
    }
}
