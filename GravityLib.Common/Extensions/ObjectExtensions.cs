using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GravityLib.Common.Extensions;

public static class ObjectExtensions
{
    /// <summary>
    /// Shorthand for serializing an object into JSON string
    /// </summary>
    /// <param name="obj">The object to be serialized</param>
    /// <param name="enumAsString">
    /// By default, property names and dictionary keys are unchanged in the JSON output, including case. Enum values are represented as numbers.
    /// <br/>Use this to support IDictionary&lt;enum, string&gt; and enum as string serialization and deserialization.
    /// </param>
    public static string ToJson(this object obj, bool enumAsString = true)
    {
        var settings = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = null, // This option turns camelCase json return type to PascalCase.
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };

        if (enumAsString)
        {
            // By default, property names and dictionary keys are unchanged in the JSON output, including case. Enum values are represented as numbers.
            // Use this to support IDictionary<enum, string> and enum as string serialization and deserialization.
            settings.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        return JsonSerializer.Serialize(obj, settings);
    }

    /// <summary>
    /// Run validation by the data annotations within the object model and also if the model implements <see cref="IValidatableObject"/> interface.
    /// </summary>
    public static ICollection<ValidationResult> Validate(this object data)
    {
        var ctx = new ValidationContext(data);

        var errors = new List<ValidationResult>();

        return !Validator.TryValidateObject(data, ctx, errors, true)
            ? errors
            : null;
    }

    /// <summary>
    /// Run validation by the data annotations within the object model and also if the model implements <see cref="IValidatableObject"/> interface.
    /// </summary>
    public static void ValidateAndThrow(this object data)
    {
        var errors = data.Validate();

        if (errors != null)
        {
            throw new Exceptions.ValidationException(errors: errors.ToDictionary());
        }
    }

    /// <summary>
    /// Converts the result from the <see cref="Validate"/> method to Dictionary with Key-Value pair of the property name -> error message.
    /// </summary>
    public static IDictionary<string, string[]> ToDictionary(this ICollection<ValidationResult> results)
    {
        // Denormalize the list of MemberNames with their error message
        // and regroup the list by Member and list of associated error messages
        return results
            .SelectMany(res => res.MemberNames.Select(m => new { Member = m, res.ErrorMessage }))
            .GroupBy(x => x.Member, x => x.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }
}