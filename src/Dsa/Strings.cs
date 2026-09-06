namespace Dsa;

public static class Strings
{
    private static readonly Dictionary<char, char> ClosingToOpening = new()
    {
        [')'] = '(',
        ['}'] = '{',
        [']'] = '[',
    };

    public static bool ValidParentheses(string input)
    {
        var stack = new Stack<char>();

        foreach (var c in input)
        {
            if (ClosingToOpening.TryGetValue(c, out var opening))
            {
                if (stack.Count == 0 || stack.Peek() != opening)
                    return false;

                stack.Pop();
            }
            else
            {
                stack.Push(c);
            }
        }

        return stack.Count == 0;
    }

    public static bool ValidParenthesesIgnoringOtherChars(string input)
    {
        var stack = new Stack<char>();

        foreach (var c in input)
        {
            if (ClosingToOpening.ContainsValue(c))
            {
                stack.Push(c);
            }
            else if (ClosingToOpening.TryGetValue(c, out var opening))
            {
                if (stack.Count == 0 || stack.Peek() != opening)
                    return false;

                stack.Pop();
            }
        }

        return stack.Count == 0;
    }

    public static bool IsPalindrome(string input)
    {
        var left = 0;
        var right = input.Length - 1;

        while (left < right)
        {
            if (input[left] != input[right])
                return false;

            left++;
            right--;
        }

        return true;
    }
}
