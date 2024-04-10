using System.Numerics;
using StockAutomationCore.Diff;
using StockAutomationCore.DiffFormat;
using StockAutomationCore.Model;

namespace StockAutomationCore.Tests;
using StockAutomationCore;

public class HoldingsDiffUnitTest
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestHoldingsDiffEmpty()
    {
        var diff = new HoldingsDiff(
            new List<HoldingSnapshotLine> {},
            new List<HoldingSnapshotLine> {}
        );
        Assert.That(diff.HoldingsDiffLines, Is.Empty);
    }

    [Test]
    public void TestHoldingsDiffSameHoldingsNoChanges()
    {
        var holding1 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 1", "TICKER1", "CUSIP1", 100, 100, decimal.Parse("0.1"));
        var holding2 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 200, 200, decimal.Parse("0.2"));
        var diff = new HoldingsDiff(
            new List<HoldingSnapshotLine> { holding1, holding2 },
            new List<HoldingSnapshotLine> { holding1, holding2 }
         );
        Assert.Multiple(() =>
        {
            Assert.That(diff.HoldingsDiffLines.TryGetValue("CUSIP1", out var val1), Is.True);
            Assert.That(val1.Cusip, Is.EqualTo("CUSIP1"));
            Assert.That(val1.Ticker, Is.EqualTo("TICKER1"));
            Assert.That(val1.CompanyName, Is.EqualTo("Company 1"));
            Assert.That(val1.QuantityDiff, Is.EqualTo(BigInteger.Zero));
            Assert.That(val1.Old.Shares, Is.EqualTo(new BigInteger(100)));
            Assert.That(val1.New.Shares, Is.EqualTo(new BigInteger(100)));
            Assert.That(val1.Old.MarketValueUsd, Is.EqualTo(100m));
            Assert.That(val1.New.MarketValueUsd, Is.EqualTo(100m));
            Assert.That(val1.Old.Weight, Is.EqualTo(decimal.Parse("0.1")));
            Assert.That(val1.New.Weight, Is.EqualTo(decimal.Parse("0.1")));
            Assert.That(diff.HoldingsDiffLines.TryGetValue("CUSIP2", out var val2), Is.True);
            Assert.That(val2.Cusip, Is.EqualTo("CUSIP2"));
            Assert.That(val2.Ticker, Is.EqualTo("TICKER2"));
            Assert.That(val2.CompanyName, Is.EqualTo("Company 2"));
            Assert.That(val2.QuantityDiff, Is.EqualTo(BigInteger.Zero));
            Assert.That(val2.Old.Shares, Is.EqualTo(new BigInteger(200)));
            Assert.That(val2.New.Shares, Is.EqualTo(new BigInteger(200)));
            Assert.That(val2.Old.MarketValueUsd, Is.EqualTo(200m));
            Assert.That(val2.New.MarketValueUsd, Is.EqualTo(200m));
            Assert.That(val2.Old.Weight, Is.EqualTo(decimal.Parse("0.2")));
            Assert.That(val2.New.Weight, Is.EqualTo(decimal.Parse("0.2")));
        });
    }

    [Test]
    public void TestHoldingsDiffNewHoldings()
    {
        var holding1 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 1", "TICKER1", "CUSIP1", 100, 100, decimal.Parse("0.1"));
        var holding2 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 200, 200, decimal.Parse("0.2"));
        var diff = new HoldingsDiff(
            new List<HoldingSnapshotLine> { holding1 },
            new List<HoldingSnapshotLine> { holding1, holding2 }
        );
        Assert.Multiple(() =>
        {
            Assert.That(diff.HoldingsDiffLines.TryGetValue("CUSIP1", out var val1), Is.True);
            Assert.That(val1.Cusip, Is.EqualTo("CUSIP1"));
            Assert.That(val1.Ticker, Is.EqualTo("TICKER1"));
            Assert.That(val1.CompanyName, Is.EqualTo("Company 1"));
            Assert.That(val1.QuantityDiff, Is.EqualTo(BigInteger.Zero));
            Assert.That(val1.Old.Shares, Is.EqualTo(new BigInteger(100)));
            Assert.That(val1.New.Shares, Is.EqualTo(new BigInteger(100)));
            Assert.That(val1.Old.MarketValueUsd, Is.EqualTo(100m));
            Assert.That(val1.New.MarketValueUsd, Is.EqualTo(100m));
            Assert.That(val1.Old.Weight, Is.EqualTo(decimal.Parse("0.1")));
            Assert.That(val1.New.Weight, Is.EqualTo(decimal.Parse("0.1")));
            Assert.That(diff.HoldingsDiffLines.TryGetValue("CUSIP2", out var val2), Is.True);
            Assert.That(val2.Cusip, Is.EqualTo("CUSIP2"));
            Assert.That(val2.Ticker, Is.EqualTo("TICKER2"));
            Assert.That(val2.CompanyName, Is.EqualTo("Company 2"));
            Assert.That(val2.QuantityDiff, Is.EqualTo(new BigInteger(200)));
            Assert.That(val2.Old.Shares, Is.EqualTo(BigInteger.Zero));
            Assert.That(val2.New.Shares, Is.EqualTo(new BigInteger(200)));
            Assert.That(val2.Old.MarketValueUsd, Is.EqualTo(0m));
            Assert.That(val2.New.MarketValueUsd, Is.EqualTo(200m));
            Assert.That(val2.Old.Weight, Is.EqualTo(0m));
            Assert.That(val2.New.Weight, Is.EqualTo(decimal.Parse("0.2")));
        });
    }

    [Test]
    public void TestHoldingsDiffRemovedHoldings()
    {
        var holding1 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 1", "TICKER1", "CUSIP1", 100, 100, decimal.Parse("0.1"));
        var holding2 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 200, 200, decimal.Parse("0.2"));
        var diff = new HoldingsDiff(
            new List<HoldingSnapshotLine> { holding1, holding2 },
            new List<HoldingSnapshotLine> { holding1 }
        );
        Assert.Multiple(() =>
        {
            Assert.That(diff.HoldingsDiffLines.TryGetValue("CUSIP1", out var val1), Is.True);
            Assert.That(val1.Cusip, Is.EqualTo("CUSIP1"));
            Assert.That(val1.Ticker, Is.EqualTo("TICKER1"));
            Assert.That(val1.CompanyName, Is.EqualTo("Company 1"));
            Assert.That(val1.QuantityDiff, Is.EqualTo(BigInteger.Zero));
            Assert.That(val1.Old.Shares, Is.EqualTo(new BigInteger(100)));
            Assert.That(val1.New.Shares, Is.EqualTo(new BigInteger(100)));
            Assert.That(val1.Old.MarketValueUsd, Is.EqualTo(100m));
            Assert.That(val1.New.MarketValueUsd, Is.EqualTo(100m));
            Assert.That(val1.Old.Weight, Is.EqualTo(decimal.Parse("0.1")));
            Assert.That(val1.New.Weight, Is.EqualTo(decimal.Parse("0.1")));
            Assert.That(diff.HoldingsDiffLines.TryGetValue("CUSIP2", out var val2), Is.True);
            Assert.That(val2.Cusip, Is.EqualTo("CUSIP2"));
            Assert.That(val2.Ticker, Is.EqualTo("TICKER2"));
            Assert.That(val2.CompanyName, Is.EqualTo("Company 2"));
            Assert.That(val2.QuantityDiff, Is.EqualTo(new BigInteger(-200)));
            Assert.That(val2.Old.Shares, Is.EqualTo(new BigInteger(200)));
            Assert.That(val2.New.Shares, Is.EqualTo(BigInteger.Zero));
            Assert.That(val2.Old.MarketValueUsd, Is.EqualTo(200m));
            Assert.That(val2.New.MarketValueUsd, Is.EqualTo(0m));
            Assert.That(val2.Old.Weight, Is.EqualTo(decimal.Parse("0.2")));
            Assert.That(val2.New.Weight, Is.EqualTo(0m));

        });
    }

    [Test]
    public void TestHoldingsDiffChangedHoldings()
    {
        var holding1 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 1", "TICKER1", "CUSIP1", 100, 100, decimal.Parse("0.1"));
        var holding2 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 200, 200, decimal.Parse("0.2"));
        var holding3 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 300, 300, decimal.Parse("0.3"));
        var diff = new HoldingsDiff(
            new List<HoldingSnapshotLine> { holding1, holding2 },
            new List<HoldingSnapshotLine> { holding1, holding3 }
        );
        Assert.Multiple(() =>
        {
            Assert.That(diff.HoldingsDiffLines.TryGetValue("CUSIP1", out var val1), Is.True);
            Assert.That(val1.Cusip, Is.EqualTo("CUSIP1"));
            Assert.That(val1.Ticker, Is.EqualTo("TICKER1"));
            Assert.That(val1.CompanyName, Is.EqualTo("Company 1"));
            Assert.That(val1.QuantityDiff, Is.EqualTo(BigInteger.Zero));
            Assert.That(val1.Old.Shares, Is.EqualTo(new BigInteger(100)));
            Assert.That(val1.New.Shares, Is.EqualTo(new BigInteger(100)));
            Assert.That(val1.Old.MarketValueUsd, Is.EqualTo(100m));
            Assert.That(val1.New.MarketValueUsd, Is.EqualTo(100m));
            Assert.That(val1.Old.Weight, Is.EqualTo(decimal.Parse("0.1")));
            Assert.That(val1.New.Weight, Is.EqualTo(decimal.Parse("0.1")));
            Assert.That(diff.HoldingsDiffLines.TryGetValue("CUSIP2", out var val2), Is.True);
            Assert.That(val2.Cusip, Is.EqualTo("CUSIP2"));
            Assert.That(val2.Ticker, Is.EqualTo("TICKER2"));
            Assert.That(val2.CompanyName, Is.EqualTo("Company 2"));
            Assert.That(val2.QuantityDiff, Is.EqualTo(new BigInteger(100)));
            Assert.That(val2.Old.Shares, Is.EqualTo(new BigInteger(200)));
            Assert.That(val2.New.Shares, Is.EqualTo(new BigInteger(300)));
            Assert.That(val2.Old.MarketValueUsd, Is.EqualTo(200m));
            Assert.That(val2.New.MarketValueUsd, Is.EqualTo(300m));
            Assert.That(val2.Old.Weight, Is.EqualTo(decimal.Parse("0.2")));
            Assert.That(val2.New.Weight, Is.EqualTo(decimal.Parse("0.3")));
        });
    }

    [Test]
    public void TestHoldingsDiffEverythingAtOnce()
    {
        var holding1 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 1", "TICKER1", "CUSIP1", 100, 100, decimal.Parse("0.1"));
        var holding2 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 200, 200, decimal.Parse("0.2"));
        var holding3 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 2", "TICKER2", "CUSIP2", 300, 300, decimal.Parse("0.3"));
        var holding4 = HoldingSnapshotLine.Create(DateTime.Today, "ARKK", "Company 3", "TICKER3", "CUSIP3", 400, 400, decimal.Parse("0.4"));

        var diff = new HoldingsDiff(
            new List<HoldingSnapshotLine> { holding1, holding2 },
            new List<HoldingSnapshotLine> { holding3, holding4 }
        );
        Assert.Multiple(() =>
        {
            Assert.That(diff.HoldingsDiffLines.TryGetValue("CUSIP1", out var val1), Is.True);
            Assert.That(val1.Cusip, Is.EqualTo("CUSIP1"));
            Assert.That(val1.Ticker, Is.EqualTo("TICKER1"));
            Assert.That(val1.CompanyName, Is.EqualTo("Company 1"));
            Assert.That(val1.QuantityDiff, Is.EqualTo(new BigInteger(-100)));
            Assert.That(val1.Old.Shares, Is.EqualTo(new BigInteger(100)));
            Assert.That(val1.New.Shares, Is.EqualTo(BigInteger.Zero));
            Assert.That(val1.Old.MarketValueUsd, Is.EqualTo(100m));
            Assert.That(val1.New.MarketValueUsd, Is.EqualTo(0m));
            Assert.That(val1.Old.Weight, Is.EqualTo(decimal.Parse("0.1")));
            Assert.That(val1.New.Weight, Is.EqualTo(0m));
            Assert.That(diff.HoldingsDiffLines.TryGetValue("CUSIP2", out var val2), Is.True);
            Assert.That(val2.Cusip, Is.EqualTo("CUSIP2"));
            Assert.That(val2.Ticker, Is.EqualTo("TICKER2"));
            Assert.That(val2.CompanyName, Is.EqualTo("Company 2"));
            Assert.That(val2.QuantityDiff, Is.EqualTo(new BigInteger(100)));
            Assert.That(val2.Old.Shares, Is.EqualTo(new BigInteger(200)));
            Assert.That(val2.New.Shares, Is.EqualTo(new BigInteger(300)));
            Assert.That(val2.Old.MarketValueUsd, Is.EqualTo(200m));
            Assert.That(val2.New.MarketValueUsd, Is.EqualTo(300m));
            Assert.That(val2.Old.Weight, Is.EqualTo(decimal.Parse("0.2")));
            Assert.That(val2.New.Weight, Is.EqualTo(decimal.Parse("0.3")));
            Assert.That(diff.HoldingsDiffLines.TryGetValue("CUSIP3", out var val3), Is.True);
            Assert.That(val3.Cusip, Is.EqualTo("CUSIP3"));
            Assert.That(val3.Ticker, Is.EqualTo("TICKER3"));
            Assert.That(val3.CompanyName, Is.EqualTo("Company 3"));
            Assert.That(val3.QuantityDiff, Is.EqualTo(new BigInteger(400)));
            Assert.That(val3.Old.Shares, Is.EqualTo(BigInteger.Zero));
            Assert.That(val3.New.Shares, Is.EqualTo(new BigInteger(400)));
            Assert.That(val3.Old.MarketValueUsd, Is.EqualTo(0m));
            Assert.That(val3.New.MarketValueUsd, Is.EqualTo(400m));
            Assert.That(val3.Old.Weight, Is.EqualTo(0m));
            Assert.That(val3.New.Weight, Is.EqualTo(decimal.Parse("0.4")));
        });
    }

}