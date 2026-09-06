namespace Tasks.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using Tasks;
using Xunit;

public class InvoiceReconciliationTests
{
    // ---------- Parser first: it's the boundary, so it gets the most cases ----------

    [Fact]
    public void ParseInvoice_HappyPath()
    {
        var inv = LineParser.ParseInvoice("I1|FundName1|2026-10-05|500");

        Assert.Equal("I1", inv.Id);
        Assert.Equal("FundName1", inv.Counterparty);
        Assert.Equal(new DateOnly(2026, 10, 5), inv.Date);
        Assert.Equal(500m, inv.Amount);
    }

    [Theory]
    [InlineData("")]                             // empty
    [InlineData("I1|FundName1|2026-10-05")]      // missing field
    [InlineData("I1|FundName1|05/10/2026|500")]  // wrong date format
    [InlineData("I1|FundName1|2026-10-05|5OO")]  // letter O, not zero
    [InlineData("I1|FundName1|2026-13-01|500")]  // month 13
    public void ParseInvoice_BadInput_ThrowsFormatException(string line)
        => Assert.Throws<FormatException>(() => LineParser.ParseInvoice(line));

    [Fact]
    public void ParsePayment_TrimsWhitespaceAndKeepsDecimals()
    {
        var p = LineParser.ParsePayment(" P1 | FundName1 | 2026-10-04 | 199.99 ");
        Assert.Equal("P1", p.Id);
        Assert.Equal(199.99m, p.Amount);
    }

    // ---------- The interview sample ----------

    private static readonly List<string> SampleInvoices = new() { "I1|FundName1|2026-10-05|500" };
    private static readonly List<string> SamplePayments = new()
    {
        "P1|FundName1|2026-10-04|200",
        "P2|FundName1|2026-10-06|200",
    };

    [Fact]
    public void Run_InterviewSample_MatchesExpectedOutput()
    {
        var result = InvoiceReconciliation.Run(SampleInvoices, SamplePayments).Single();

        Assert.Equal("I1", result.InvoiceId);
        Assert.Equal(new[] { "P1", "P2" }, result.PaymentIds);
        Assert.Equal(100m, result.Remaining);
        Assert.Equal("I1 : [P1, P2] | remaining: 100", result.ToString());
    }

    // ---------- Matching rules ----------

    [Fact]
    public void Match_DateWindowIsInclusiveOnBothEdges()
    {
        var inv = new[] { new Invoice("I1", "F", new DateOnly(2026, 10, 5), 100m) };
        var pay = new[]
        {
            new Payment("P-minus1", "F", new DateOnly(2026, 10, 4), 10m),
            new Payment("P-plus1",  "F", new DateOnly(2026, 10, 6), 10m),
            new Payment("P-plus2",  "F", new DateOnly(2026, 10, 7), 10m),  // outside
        };

        var r = InvoiceMatcher.MatchIndexed(inv, pay).Single();

        Assert.Equal(new[] { "P-minus1", "P-plus1" }, r.PaymentIds);
        Assert.Equal(80m, r.Remaining);
    }

    [Fact]
    public void Match_DifferentCounterpartyIsIgnored()
    {
        var inv = new[] { new Invoice("I1", "F1", new DateOnly(2026, 10, 5), 100m) };
        var pay = new[] { new Payment("P1", "F2", new DateOnly(2026, 10, 5), 100m) };

        var r = InvoiceMatcher.MatchIndexed(inv, pay).Single();

        Assert.Empty(r.PaymentIds);
        Assert.Equal(100m, r.Remaining);
    }

    [Fact]
    public void Match_InvoiceWithNoPaymentsForFund_DoesNotThrow()
    {
        var inv = new[] { new Invoice("I1", "Unknown", new DateOnly(2026, 10, 5), 100m) };
        var r = InvoiceMatcher.MatchIndexed(inv, Array.Empty<Payment>()).Single();
        Assert.Equal(100m, r.Remaining);
    }

    [Fact]
    public void Match_OverpaymentGivesNegativeRemaining()
    {
        var inv = new[] { new Invoice("I1", "F", new DateOnly(2026, 10, 5), 100m) };
        var pay = new[] { new Payment("P1", "F", new DateOnly(2026, 10, 5), 150m) };

        var r = InvoiceMatcher.MatchIndexed(inv, pay).Single();
        Assert.Equal(-50m, r.Remaining);
    }

    // ---------- The difference between brute force and indexed ----------

    [Fact]
    public void BruteForce_SamePaymentCountedTwice_Indexed_AllocatesOnce()
    {
        // Two invoices, same fund, consecutive days: one payment sits in both windows.
        var inv = new[]
        {
            new Invoice("I1", "F", new DateOnly(2026, 10, 5), 100m),
            new Invoice("I2", "F", new DateOnly(2026, 10, 6), 100m),
        };
        var pay = new[] { new Payment("P1", "F", new DateOnly(2026, 10, 5), 100m) };

        var brute = InvoiceMatcher.MatchBruteForce(inv, pay);
        var indexed = InvoiceMatcher.MatchIndexed(inv, pay);

        // Brute force: P1 appears under both invoices — 100 paid, 200 "settled".
        Assert.Equal(0m, brute.Single(r => r.InvoiceId == "I1").Remaining);
        Assert.Equal(0m, brute.Single(r => r.InvoiceId == "I2").Remaining);

        // Indexed: earliest invoice claims P1, second stays open.
        Assert.Equal(0m,   indexed.Single(r => r.InvoiceId == "I1").Remaining);
        Assert.Equal(100m, indexed.Single(r => r.InvoiceId == "I2").Remaining);
    }
}
