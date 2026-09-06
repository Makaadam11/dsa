namespace Tasks.Tests;

using System.Collections.Generic;
using Tasks;
using Xunit;

public class EasyDsaTests
{
    // ---------- ISIN ----------
    [Theory]
    [InlineData("GB0002634946", true)]   // valid
    [InlineData("gb0002634946", false)]  // lowercase country code
    [InlineData("GB00026349", false)]    // too short
    [InlineData("GB00026349AB", false)]  // check digit is a letter
    [InlineData("GB@002634946", false)]  // bad char exactly at index 2 (the boundary that bit us)
    [InlineData("", false)]
    public void IsValidIsin_Cases(string input, bool expected)
        => Assert.Equal(expected, EasyDsa.IsValidIsin(input));

    // ---------- Anagrams ----------
    [Theory]
    [InlineData("mama", "amam", true)]
    [InlineData("mama", "axam", false)]
    [InlineData("Listen", "Silent", true)]  // case-insensitive
    [InlineData("ab", "abc", false)]        // extra char in b — the length-gate bug
    [InlineData("aab", "abb", false)]       // same length, different counts
    public void Anagrams_BothVersionsAgree(string a, string b, bool expected)
    {
        Assert.Equal(expected, EasyDsa.AreAnagramsSort(a, b));
        Assert.Equal(expected, EasyDsa.AreAnagramsDict(a, b));
    }

    // ---------- Dictionary counting ----------
    [Fact]
    public void CountTransactionsAboveThreshold_FiltersAndGroups()
    {
        var tx = new List<(string, decimal)>
        {
            ("fund1", 150m), ("fund1", 50m), ("fund1", 200m),
            ("fund2", 99m),  ("fund2", 101m),
        };

        var result = EasyDsa.CountTransactionsAboveThreshold(tx, 100m);

        Assert.Equal(2, result["fund1"]);
        Assert.Equal(1, result["fund2"]);
        Assert.Equal(2, result.Count);   // fund with no qualifying tx is absent, not zero
    }

    // ---------- Valid parentheses ----------
    [Theory]
    [InlineData("()[]", true)]
    [InlineData("([)]", false)]   // wrong nesting
    [InlineData("(", false)]      // unclosed opener
    [InlineData(")", false)]      // closer on empty stack — the silent-ignore bug
    [InlineData("())", false)]    // extra closer mid-string
    [InlineData("", true)]        // empty input is valid
    [InlineData("({[]})", true)]
    public void IsValidParentheses_Cases(string input, bool expected)
        => Assert.Equal(expected, EasyDsa.IsValidParentheses(input));
}
