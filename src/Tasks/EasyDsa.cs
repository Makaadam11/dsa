namespace Tasks;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Easy DSA tasks — final reference solutions (interview prep, eFront pairing 26 Aug).
/// Pattern notes are in comments — read them aloud when reviewing.
/// </summary>
public static class EasyDsa
{
    // ============================================================
    // TASK 1 — ISIN format validation (from a real BlackRock interview)
    // Pattern: length guard → for loop (position matters) → fail-fast → return true AFTER the loop
    // ============================================================
    public static bool IsValidIsin(string s)
    {
        if (string.IsNullOrEmpty(s) || s.Length != 12)
            return false;

        for (int i = 0; i < 12; i++)
        {
            char c = s[i];
            if (i < 2)
            {
                // IsUpper('5') is false anyway — no need for an extra IsLetter check
                if (!char.IsUpper(c)) return false;
            }
            else if (i < 11)   // positions 2..10 — watch the boundary: >= 2 covered by else-if order
            {
                if (!char.IsLetterOrDigit(c)) return false;
            }
            else               // position 11 — check digit
            {
                if (!char.IsDigit(c)) return false;
            }
            // no return on success inside the loop — success is only known AFTER all chars pass
        }
        return true;
    }

    // ============================================================
    // TASK 2a — Anagrams via sort. O(n log n), three lines, hard to get wrong.
    // Say aloud: "I'll start with the simple sort-based version, then offer the O(n) one."
    // ============================================================
    public static bool AreAnagramsSort(string a, string b)
    {
        if (a.Length != b.Length) return false;
        return string.Concat(a.ToLower().OrderBy(c => c))
            == string.Concat(b.ToLower().OrderBy(c => c));
    }

    // ============================================================
    // TASK 2b — Anagrams via dictionary. O(n).
    // Pattern: count UP on a, count DOWN on b. Length gate handles extra chars in b.
    // Bug this fixes: without the length check, "ab" vs "abc" returns true
    // (chars in b that never appear in a are otherwise never validated).
    // ============================================================
    public static bool AreAnagramsDict(string a, string b)
    {
        if (a.Length != b.Length) return false;

        a = a.ToLower();
        b = b.ToLower();
        var counts = new Dictionary<char, int>();

        foreach (char c in a)
            counts[c] = counts.GetValueOrDefault(c) + 1;

        foreach (char c in b)
        {
            if (!counts.TryGetValue(c, out int n) || n == 0) return false;
            counts[c] = n - 1;
        }
        return true;
    }

    // ============================================================
    // TASK 3 — Dictionary counting with LINQ.
    // Pipeline order: filter → group → materialize. Sum/Count END a pipeline.
    // ============================================================
    public static Dictionary<string, int> CountTransactionsAboveThreshold(
        List<(string Fund, decimal Amount)> transactions, decimal threshold)
    {
        return transactions
            .Where(t => t.Amount > threshold)
            .GroupBy(t => t.Fund)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    // ============================================================
    // TASK 4 — Valid parentheses with a Stack.
    // Bug this fixes: a closer on an EMPTY stack must fail immediately —
    // silently ignoring it makes ")" and "())" return true.
    // Interview question to ask: "can the input contain non-bracket characters?"
    // ============================================================
    public static bool IsValidParentheses(string s)
    {
        var stack = new Stack<char>();
        var pairs = new Dictionary<char, char> { { ')', '(' }, { ']', '[' }, { '}', '{' } };

        foreach (char c in s)
        {
            if (!pairs.ContainsKey(c))
            {
                stack.Push(c);                    // opener (or any other char, per task spec)
            }
            else
            {
                // closer: empty stack = nothing to match = fail fast
                if (stack.Count == 0 || stack.Pop() != pairs[c]) return false;
            }
        }
        return stack.Count == 0;                  // leftovers like "(" must fail too
    }
}
