using System;
using System.Linq;
using System.Reflection;

namespace GravityLib.Common.Extensions;

public static class TypeExtensions
{
    public static bool IsImplementationOf(this Type type, Type interfaceType)
    {
        return type.GetInterfaces().Contains(interfaceType);
    }

    public static bool IsImplementationOf<T>(this Type type)
    {
        return type.GetInterfaces().Contains(typeof(T));
    }

    public static bool HasAttribute<T>(this Type type) where T : Attribute
    {
        return Attribute.IsDefined(type, typeof(T));
    }

    public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
    {
        return Attribute.IsDefined(memberInfo, typeof(T));
    }

    public static bool HasAttribute<T>(this ParameterInfo parameterInfo) where T : Attribute
    {
        return Attribute.IsDefined(parameterInfo, typeof(T));
    }
}