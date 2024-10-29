using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace GravityLib.EntityFramework.Attributes
{
    /// <summary>
    /// Configures the default value for the column that the property maps to when targeting a relational database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DefaultValueAttribute : BaseEFModelPropertyAttribute
    {
        private readonly object _value;

        public DefaultValueAttribute(object value)
        {
            _value = value;
        }

        public override void ApplyToModelBuilder(Type type, PropertyInfo propertyInfo, ModelBuilder builder)
        {
            builder.Entity(type)
                .Property(propertyInfo.Name)
                .HasDefaultValue(_value);
        }
    }
}
