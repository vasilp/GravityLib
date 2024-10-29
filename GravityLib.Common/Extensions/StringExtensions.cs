using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GravityLib.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Splits sentence into words. Can handle single and double-quoted text.
    /// </summary>
    public static List<string> SplitIntoWords(this string input)
    {
        var pattern = @"(?<=\s|^)(""[^""]*""|'[^']*'|[^\s""']+)(?=\s|$)";
        var regex = new Regex(pattern);
        var matches = regex.Matches(input);

        var words = new List<string>();
        foreach (Match match in matches)
        {
            var word = match.Value.Trim('\'', '"');
            words.Add(word);
        }

        return words;
    }

    /// <summary>
    /// Strips non-printable chars and white-spaces except \r\n\t
    /// https://stackoverflow.com/questions/40564692/c-sharp-regex-to-remove-non-printable-characters-and-control-characters-in-a
    /// </summary>
    public static string StripNonPrintableCharacters(this string input)
    {
        return input != null
            ? Regex.Replace(input, @"[\p{C}-[\r\n\t]]+", string.Empty)
            : null;
    }
}