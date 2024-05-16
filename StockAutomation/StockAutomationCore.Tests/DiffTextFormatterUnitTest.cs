using StockAutomationCore.Diff;
using StockAutomationCore.DiffFormat;
using StockAutomationCore.Model;

namespace StockAutomationCore.Tests;

public class TextFormatterUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestHoldingsDiffToTextEmptyEmptyNoChanges()
    {
        var diff = new HoldingsDiff(
            new List<HoldingSnapshotLine>(),
            new List<HoldingSnapshotLine>()
        );
        const string expected = "No changes in the index";
        Assert.That(TextDiffFormatter.FormatText(diff), Is.EqualTo(expected));
    }

    [Test]
    public void TestHoldingsDiffToTextSameHoldingsNoChanges()
    {
        var holding1 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 1", "TICKER1", "CUSIP1", 100, 100,
            decimal.Parse("0.1"));
        var holding2 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 200, 200,
            decimal.Parse("0.2"));
        var diff = new HoldingsDiff(
            new List<HoldingSnapshotLine> { holding1, holding2 },
            new List<HoldingSnapshotLine> { holding1, holding2 }
        );
        const string expected = "No changes in the index";
        Assert.That(TextDiffFormatter.FormatText(diff), Is.EqualTo(expected));
    }

    [Test]
    public void TestHoldingsDiffToTextNewHoldings()
    {
        var holding1 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 1", "TICKER1", "CUSIP1", 100, 100,
            decimal.Parse("0.1"));
        var holding2 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 200, 200,
            decimal.Parse("0.2"));
        var diff = new HoldingsDiff(
            new List<HoldingSnapshotLine> { holding1 },
            new List<HoldingSnapshotLine> { holding1, holding2 }
        );
        const string expected = "New positions:\r\nCompany 2, TICKER2, 200, 20.00%";
        Assert.That(TextDiffFormatter.FormatText(diff), Is.EqualTo(expected));
    }

    [Test]
    public void TestHoldingsDiffToTextRemovedHoldings()
    {
        var holding1 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 1", "TICKER1", "CUSIP1", 100, 100,
            decimal.Parse("0.1"));
        var holding2 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 200, 200,
            decimal.Parse("0.2"));
        var diff = new HoldingsDiff(
            new List<HoldingSnapshotLine> { holding1, holding2 },
            new List<HoldingSnapshotLine> { holding1 }
        );
        const string expected = "Reduced positions:\r\nCompany 2, TICKER2, 0 (📉100.00%), 0.00%";
        Assert.That(TextDiffFormatter.FormatText(diff), Is.EqualTo(expected));
    }

    [Test]
    public void TestHoldingsDiffToTextChangedHoldings()
    {
        var holding1 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 1", "TICKER1", "CUSIP1", 100, 100,
            decimal.Parse("0.1"));
        var holding2 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 200, 200,
            decimal.Parse("0.2"));
        var holding3 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 300, 300,
            decimal.Parse("0.3"));
        var diff = new HoldingsDiff(
            new List<HoldingSnapshotLine> { holding1, holding2 },
            new List<HoldingSnapshotLine> { holding1, holding3 }
        );
        const string expected = "Increased positions:\r\nCompany 2, TICKER2, 300 (📈50.00%), 30.00%";
        Assert.That(TextDiffFormatter.FormatText(diff), Is.EqualTo(expected));
    }

    [Test]
    public void TestHoldingsDiffToTextEverythingAtOnce()
    {
        var holding1 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 1", "TICKER1", "CUSIP1", 100, 100,
            decimal.Parse("0.1"));
        var holding2 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 200, 200,
            decimal.Parse("0.2"));
        var holding3 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 300, 300,
            decimal.Parse("0.3"));
        var holding4 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 3", "TICKER3", "CUSIP3", 400, 400,
            decimal.Parse("0.4"));

        var diff = new HoldingsDiff(
            new List<HoldingSnapshotLine> { holding1, holding2 },
            new List<HoldingSnapshotLine> { holding3, holding4 }
        );
        const string expected =
            "New positions:\r\nCompany 3, TICKER3, 400, 40.00%\r\n\r\nIncreased positions:\r\nCompany 2, TICKER2, 300 (📈50.00%), 30.00%\r\n\r\nReduced positions:\r\nCompany 1, TICKER1, 0 (📉100.00%), 0.00%";
        Assert.That(TextDiffFormatter.FormatText(diff), Is.EqualTo(expected));
    }
}
