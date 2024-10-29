using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GravityLib.EntityFramework.Attributes
{
    /// <summary>
    /// Specifies an index to be generated in the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ComplexKeyAttribute : BaseEFModelAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexKeyAttribute" /> class. </summary>
        /// <param name="propertyNames"> The properties which constitute the primary key, in order (there must be at least one). </param>
        public ComplexKeyAttribute(params string[] propertyNames)
        {
            if (propertyNames == null || propertyNames.Length == 0)
                throw new ArgumentException(null, nameof(propertyNames));

            if (propertyNames.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("ComplexKeyAttribute with " + nameof(propertyNames) + " cannot contain empty names!");

            PropertyNames = propertyNames;
        }

        public string[] PropertyNames { get; }

        public override void ApplyToModelBuilder(Type t, ModelBuilder builder)
        {
            builder.Entity(t).HasKey(PropertyNames);
        }
    }
}
