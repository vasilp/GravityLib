using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace GravityLib.EntityFramework.Attributes
{
    public abstract class BaseEFModelPropertyAttribute : Attribute
    {
        /// <summary>
        /// Apply the custom class attribute to EF model builder.
        /// </summary>
        /// <param name="type">The entity type to where the property resides.</param>
        /// <param name="propertyInfo">The property to which an attribute is being applied.</param>
        /// <param name="builder">The EF model builder.</param>
        public abstract void ApplyToModelBuilder(Type type, PropertyInfo propertyInfo, ModelBuilder builder);
    }
}
