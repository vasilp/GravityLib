using System;
using Microsoft.EntityFrameworkCore;

namespace GravityLib.EntityFramework.Attributes
{
    public abstract class BaseEFModelAttribute : Attribute
    {
        /// <summary>
        /// Apply the custom class attribute to EF model builder.
        /// </summary>
        /// <param name="type">The entity type to which an attribute is being applied.</param>
        /// <param name="builder">The EF model builder.</param>
        public abstract void ApplyToModelBuilder(Type type, ModelBuilder builder);
    }
}
