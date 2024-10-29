using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GravityLib.Common.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Tries to extract the human readable display text (description) for the given enum item
    /// </summary>
    public static string GetDescription<TEnum>(this TEnum value)
        where TEnum : Enum
    {
        var field = value.GetType().GetField(value.ToString());

        return field.GetCustomAttribute<DescriptionAttribute>()?.Description
               ?? field.GetCustomAttribute<DisplayAttribute>()?.Description
               ?? field.Name;
    }

    /// <summary>
    /// Returns all human readable display texts (descriptions) of the items in the given enum type (via generic)
    /// </summary>
    public static IList<string> GetDescriptionsOf<TEnum>()
        where TEnum : Enum
    {
        var allEnumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        return allEnumValues
            .Select(item => item.GetDescription())
            .ToList();
    }

    public static Expression<Func<TSource, int>> DescriptionOrderer<TSource, TEnum>(this Expression<Func<TSource, TEnum>> source)
        where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        if (!enumType.IsEnum)
            throw new InvalidOperationException();

        var body = ((TEnum[])Enum.GetValues(enumType))
            .OrderBy(value => value.GetDescription())
            .Select((value, ordinal) => new { value, ordinal })
            .Reverse()
            .Aggregate((Expression)null, (next, item) => next == null
                ? Expression.Constant(item.ordinal)
                : Expression.Condition(
                    Expression.Equal(source.Body, Expression.Constant(item.value)),
                    Expression.Constant(item.ordinal),
                    next));

        return Expression.Lambda<Func<TSource, int>>(body, source.Parameters[0]);
    }

    public static Dictionary<string, string> ToDictionary<TEnum>()
        where TEnum : Enum
    {
        var enums = Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .ToList();

        return enums.ToDictionary(v => v.ToString(), v => v.GetDescription());
    }
}