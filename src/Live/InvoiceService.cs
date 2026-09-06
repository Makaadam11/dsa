namespace Live;


public class InvoiceService
{
    static List<string> Invoices = new()
    {
        "I1|FundName1|2026-10-05|500",
    };

    static List<string> Payments = new()
    {
        "P1|FundName1|2026-10-04|200",
        "P2|FundName1|2026-10-06|200",
    };

    // Expected output:
    // I1 : [P1, P2] | remaining: 100

    public static void Main()
    {

    }
}
