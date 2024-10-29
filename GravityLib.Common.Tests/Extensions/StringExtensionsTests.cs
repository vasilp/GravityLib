using GravityLib.Common.Extensions;
using Xunit;

namespace GravityLib.Common.Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("hello", new string[] { "hello" })]
    [InlineData("hello world", new string[] { "hello", "world" })]
    [InlineData("\"hello world\" 'goodbye world'", new string[] { "hello world", "goodbye world" })]
    [InlineData("hello 'goodbye world'", new string[] { "hello", "goodbye world" })]
    [InlineData("", new string[] { })]
    [InlineData("   ", new string[] { })]
    [InlineData("\"hello   world\" 'goodbye   world'", new string[] { "hello   world", "goodbye   world" })]
    [InlineData("hello   world", new string[] { "hello", "world" })]
    [InlineData("hello 'goodbye   world'", new string[] { "hello", "goodbye   world" })]
    [InlineData("Axis 4K HikVision", new string[] { "Axis", "4K", "HikVision" })]
    public void SplitIntoWords_ReturnsExpectedResult(string input, string[] expected)
    {
        // Act
        var actual = input.SplitIntoWords();

        // Assert
        Assert.Equal(expected, actual);
    }

    private const string jsonTest = "{\"prop1\":\"abcаяЩУöôœü\",\"prop2\":[]}";

    [Theory]
    [InlineData("abc", "abc")]
    [InlineData("a\nb\tc\rd", "a\nb\tc\rd")]
    [InlineData("ab c°®©™", "ab c°®©™")]
    [InlineData(jsonTest, jsonTest)]
    [InlineData("★", "★")]
    [InlineData(null, null)]
    public void StripNonPrintableCharacters_ReturnsExpectedResult(string input, string expected)
    {
        // Act
        var actual = input.StripNonPrintableCharacters();

        // Assert
        Assert.Equal(expected, actual);
    }
}
