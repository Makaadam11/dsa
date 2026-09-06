namespace Tasks;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

/// <summary>
/// LIVE TASK — BlackRock eFront Invest pairing, 26 Aug 2026 (Bouvet / Milosevic).
///
/// Input: pipe-delimited strings   "I1|FundName1|2026-10-05|500"   (invoices)
///                                "P1|FundName1|2026-10-04|200"   (payments)
/// Goal:  for every invoice, find payments from the same counterparty dated within
///        ±1 day of the invoice, and report the matched payment ids + remaining amount.
/// Expected: I1 : [P1, P2] | remaining: 100
///
/// This file is the "how it should be done" reference. The Live/ project keeps the
/// interview-style brute-force version (rewrite it there from memory, timed).
///
/// Follow-up questions they asked, and where the answer lives in this file:
///   "how would you change the field types?"     -> the records (decimal + DateOnly, never double/string)
///   "how would you parse it better?"            -> LineParser (TryParse + line context in errors)
///   "how would you divide it?"                  -> parse / match / format are three separate types
///   "what is the most critical part to test?"   -> the parser. It is the boundary between untrusted
///                                                  text and the domain; every downstream bug starts there.
/// </summary>

// ---------- Domain ----------
// decimal for money (double gives 0.1 + 0.2 != 0.3), DateOnly for a calendar date with no time zone.
// Records: immutable, value equality, one-line definition — say "records" out loud, interviewers like it.
public record Invoice(string Id, string Counterparty, DateOnly Date, decimal Amount);
public record Payment(string Id, string Counterparty, DateOnly Date, decimal Amount);

public record InvoiceMatch(string InvoiceId, IReadOnlyList<string> PaymentIds, decimal Remaining)
{
    public override string ToString()
        => $"{InvoiceId} : [{string.Join(", ", PaymentIds)}] | remaining: {Remaining}";
}

// ---------- Parsing ----------
public static class LineParser
{
    private const char Separator = '|';
    private const string DateFormat = "yyyy-MM-dd";

    public static Invoice ParseInvoice(string line)
    {
        var (id, cp, date, amount) = ParseFields(line);
        return new Invoice(id, cp, date, amount);
    }

    public static Payment ParsePayment(string line)
    {
        var (id, cp, date, amount) = ParseFields(line);
        return new Payment(id, cp, date, amount);
    }

    // One private method does the work; the public ones just pick the record type.
    // Always InvariantCulture: "500" parses fine everywhere, "1,5" does not.
    private static (string Id, string Counterparty, DateOnly Date, decimal Amount) ParseFields(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            throw new FormatException("Empty line.");

        var parts = line.Split(Separator);
        if (parts.Length != 4)
            throw new FormatException($"Expected 4 fields, got {parts.Length}: '{line}'");

        if (!DateOnly.TryParseExact(parts[2].Trim(), DateFormat, CultureInfo.InvariantCulture,
                                    DateTimeStyles.None, out var date))
            throw new FormatException($"Bad date '{parts[2]}' in '{line}'");

        if (!decimal.TryParse(parts[3].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
            throw new FormatException($"Bad amount '{parts[3]}' in '{line}'");

        return (parts[0].Trim(), parts[1].Trim(), date, amount);
    }
}

// ---------- Matching ----------
public static class InvoiceMatcher
{
    /// <summary>
    /// Version A — what I wrote in the interview. O(invoices × payments), LINQ per invoice.
    /// Correct for the sample; over-matches when one payment could satisfy two invoices
    /// (same fund, overlapping windows) because nothing marks a payment as consumed.
    /// </summary>
    public static List<InvoiceMatch> MatchBruteForce(
        IEnumerable<Invoice> invoices, IEnumerable<Payment> payments, int toleranceDays = 1)
    {
        var paymentList = payments.ToList();
        var results = new List<InvoiceMatch>();

        foreach (var invoice in invoices)
        {
            var from = invoice.Date.AddDays(-toleranceDays);
            var to   = invoice.Date.AddDays(toleranceDays);

            var matched = paymentList
                .Where(p => p.Counterparty == invoice.Counterparty
                         && p.Date >= from && p.Date <= to)
                .ToList();

            var remaining = invoice.Amount - matched.Sum(p => p.Amount);   // Sum ends the chain
            results.Add(new InvoiceMatch(invoice.Id, matched.Select(p => p.Id).ToList(), remaining));
        }

        return results;
    }

    /// <summary>
    /// Version B — the "how would you improve it" answer.
    /// 1. Index payments by counterparty once: O(P) build, then each invoice only scans its own fund.
    /// 2. Track consumed payments so a payment is allocated to at most one invoice.
    ///    Invoices are processed in date order so the earliest invoice gets first claim.
    /// </summary>
    public static List<InvoiceMatch> MatchIndexed(
        IEnumerable<Invoice> invoices, IEnumerable<Payment> payments, int toleranceDays = 1)
    {
        var byCounterparty = payments
            .GroupBy(p => p.Counterparty)
            .ToDictionary(g => g.Key, g => g.OrderBy(p => p.Date).ToList());

        var consumed = new HashSet<string>();
        var results = new List<InvoiceMatch>();

        foreach (var invoice in invoices.OrderBy(i => i.Date))
        {
            var from = invoice.Date.AddDays(-toleranceDays);
            var to   = invoice.Date.AddDays(toleranceDays);

            // TryGetValue, not indexer — an invoice with no payments must not throw.
            var candidates = byCounterparty.TryGetValue(invoice.Counterparty, out var list)
                ? list
                : new List<Payment>();

            var matched = new List<Payment>();
            foreach (var p in candidates)
            {
                if (p.Date < from || p.Date > to) continue;
                if (!consumed.Add(p.Id)) continue;      // already used by an earlier invoice
                matched.Add(p);
            }

            var remaining = invoice.Amount - matched.Sum(p => p.Amount);
            results.Add(new InvoiceMatch(invoice.Id, matched.Select(p => p.Id).ToList(), remaining));
        }

        return results;
    }
}

// ---------- Orchestration ----------
public static class InvoiceReconciliation
{
    public static List<InvoiceMatch> Run(IEnumerable<string> invoiceLines, IEnumerable<string> paymentLines)
    {
        var invoices = invoiceLines.Select(LineParser.ParseInvoice).ToList();
        var payments = paymentLines.Select(LineParser.ParsePayment).ToList();
        return InvoiceMatcher.MatchIndexed(invoices, payments);
    }
}
