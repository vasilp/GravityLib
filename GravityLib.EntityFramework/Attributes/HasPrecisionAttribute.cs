using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace GravityLib.EntityFramework.Attributes
{
    /// <summary>
    /// Configures the default value for the column that the property maps to when targeting a relational database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class HasPrecisionAttribute : BaseEFModelPropertyAttribute
    {
        private readonly int _precision;
        private readonly int? _scale;

        public HasPrecisionAttribute(int precision)
        {
            _precision = precision;
        }

        public HasPrecisionAttribute(int precision, int scale)
        {
            _precision = precision;
            _scale = scale;
        }

        public override void ApplyToModelBuilder(Type type, PropertyInfo propertyInfo, ModelBuilder builder)
        {
            var p = builder.Entity(type).Property(propertyInfo.Name);

            if (_scale.HasValue)
                p.HasPrecision(_precision, _scale.Value);
            else
                p.HasPrecision(_precision);
        }
    }
}
