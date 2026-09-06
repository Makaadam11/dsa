using Dsa;

namespace Dsa.Tests;

public class StringsTests
{
    [Theory]
    [InlineData("racecar", true)]
    [InlineData("hello", false)]
    [InlineData("a", true)]
    [InlineData("", true)]
    [InlineData("ab", false)]
    [InlineData("aa", true)]
    [InlineData("aba", true)]
    public void IsPalindrome_Works(string input, bool expected)
        => Assert.Equal(expected, Strings.IsPalindrome(input));

    [Theory]
    [InlineData("()", true)]
    [InlineData("({[]})", true)]
    [InlineData("(]", false)]
    [InlineData("(", false)]
    [InlineData(")", false)]
    [InlineData("", true)]
    public void ValidParentheses_Works(string input, bool expected)
        => Assert.Equal(expected, Strings.ValidParentheses(input));

    [Theory]
    [InlineData("a(b[c]d)e", true)]
    [InlineData("a(b]c", false)]
    [InlineData("abc", true)]
    public void ValidParenthesesIgnoringOtherChars_IgnoresNonBrackets(string input, bool expected)
        => Assert.Equal(expected, Strings.ValidParenthesesIgnoringOtherChars(input));

    [Fact]
    public void ValidParentheses_TreatsUnknownCharsAsOpening()
    {
        Assert.False(Strings.ValidParentheses("abc"));
    }
}
