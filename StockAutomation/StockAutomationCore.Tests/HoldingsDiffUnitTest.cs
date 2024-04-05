using StockAutomationCore.Diff;

namespace StockAutomationCore.Tests;
using StockAutomationCore;

public class HoldingsDiffUnitTest
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestHoldingsDiffToTextEmptyEmptyNoChanges()
    {
        var diff = new HoldingsDiff(
            new List<HoldingsSnapshotLine> {},
            new List<HoldingsSnapshotLine> {}
        );
        const string expected = "No changes in the index";
        Assert.That(diff.ToText(), Is.EqualTo(expected));
    }

    [Test]
    public void TestHoldingsDiffToTextSameHoldingsNoChanges()
    {
        var holding1 = HoldingsSnapshotLine.New("Company 1", "TICKER1", 100, 0.1f).Value;
        var holding2 = HoldingsSnapshotLine.New("Company 2", "TICKER2", 200, 0.2f).Value;
        var diff = new HoldingsDiff(
            new List<HoldingsSnapshotLine> { holding1, holding2 },
            new List<HoldingsSnapshotLine> { holding1, holding2 }
        );
        const string expected = "No changes in the index";
        Assert.That(diff.ToText(), Is.EqualTo(expected));
    }

    [Test]
    public void TestHoldingsDiffToTextNewHoldings()
    {
        var holding1 = HoldingsSnapshotLine.New("Company 1", "TICKER1", 100, 0.1f).Value;
        var holding2 = HoldingsSnapshotLine.New("Company 2", "TICKER2", 200, 0.2f).Value;
        var diff = new HoldingsDiff(
            new List<HoldingsSnapshotLine> { holding1 },
            new List<HoldingsSnapshotLine> { holding1, holding2 }
        );
        const string expected = "New positions:\r\nCompany 2, TICKER2, 200, 20.00%";
        Assert.That(diff.ToText(), Is.EqualTo(expected));
    }

    [Test]
    public void TestHoldingsDiffToTextRemovedHoldings()
    {
        var holding1 = HoldingsSnapshotLine.New("Company 1", "TICKER1", 100, 0.1f).Value;
        var holding2 = HoldingsSnapshotLine.New("Company 2", "TICKER2", 200, 0.2f).Value;
        var diff = new HoldingsDiff(
            new List<HoldingsSnapshotLine> { holding1, holding2 },
            new List<HoldingsSnapshotLine> { holding1 }
        );
        const string expected = "Reduced positions:\r\nCompany 2, TICKER2, 0 (🔻100.00%), 0.00%";
        Assert.That(diff.ToText(), Is.EqualTo(expected));
    }

    [Test]
    public void TestHoldingsDiffToTextChangedHoldings()
    {
        var holding1 = HoldingsSnapshotLine.New("Company 1", "TICKER1", 100, 0.1f).Value;
        var holding2 = HoldingsSnapshotLine.New("Company 2", "TICKER2", 200, 0.2f).Value;
        var holding3 = HoldingsSnapshotLine.New("Company 2", "TICKER2", 300, 0.3f).Value;
        var diff = new HoldingsDiff(
            new List<HoldingsSnapshotLine> { holding1, holding2 },
            new List<HoldingsSnapshotLine> { holding1, holding3 }
        );
        const string expected = "Increased positions:\r\nCompany 2, TICKER2, 300 (🔺50.00%), 30.00%";
        Assert.That(diff.ToText(), Is.EqualTo(expected));
    }

    [Test]
    public void TestHoldingsDiffToTextEverythingAtOnce()
    {
        var holding1 = HoldingsSnapshotLine.New("Company 1", "TICKER1", 100, 0.1f).Value;
        var holding2 = HoldingsSnapshotLine.New("Company 2", "TICKER2", 200, 0.2f).Value;
        var holding3 = HoldingsSnapshotLine.New("Company 2", "TICKER2", 300, 0.3f).Value;
        var holding4 = HoldingsSnapshotLine.New("Company 3", "TICKER3", 400, 0.4f).Value;

        var diff = new HoldingsDiff(
            new List<HoldingsSnapshotLine> { holding1, holding2 },
            new List<HoldingsSnapshotLine> {holding3, holding4 }
        );
        const string expected = "New positions:\r\nCompany 3, TICKER3, 400, 40.00%\r\n\r\nIncreased positions:\r\nCompany 2, TICKER2, 300 (🔺50.00%), 30.00%\r\n\r\nReduced positions:\r\nCompany 1, TICKER1, 0 (🔻100.00%), 0.00%";
        Assert.That(diff.ToText(), Is.EqualTo(expected));
    }

}