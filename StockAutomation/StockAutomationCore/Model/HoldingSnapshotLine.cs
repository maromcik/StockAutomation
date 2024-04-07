using System.Numerics;

namespace StockAutomationCore.Model;

public readonly struct HoldingSnapshotLine
{
    public DateTime Date { get; } // apparently no `Date` type
    public string Fund { get; }
    public string CompanyName { get; }
    public string Ticker { get; }
    public string Cusip { get; }
    public BigInteger Shares { get; }
    public decimal MarketValueUSD { get; }
    public decimal Weight { get; }

    private HoldingSnapshotLine(DateTime date, string fund, string companyName, string ticker, string cusip, BigInteger shares, decimal marketValueUSD, decimal weight)
    {
        Date = date;
        Fund = fund;
        CompanyName = companyName;
        Ticker = ticker;
        Cusip = cusip;
        Shares = shares;
        MarketValueUSD = marketValueUSD;
        Weight = weight;
    }

    private HoldingSnapshotLine(DateTime date, string fund, string companyName, string ticker, string cusip)
    {
        Date = date;
        Fund = fund;
        CompanyName = companyName;
        Ticker = ticker;
        Cusip = cusip;
        Shares = 0;
        MarketValueUSD = 0;
        Weight = 0;
    }

    public static HoldingSnapshotLine Create(DateTime date, string fund, string company, string ticker, string cusip, BigInteger shares, decimal marketValueUSD, decimal weight)
    {
        // todo assertions
        return new HoldingSnapshotLine(date, fund, company, ticker, cusip, shares, marketValueUSD, weight);
    }

    public static HoldingSnapshotLine CreateDefaultFrom(HoldingSnapshotLine other)
    {
        return new HoldingSnapshotLine(other.Date, other.Fund, other.CompanyName, other.Ticker, other.Cusip);
    }

    public override string ToString()
    {
        return $"HoldingSnapshotLine({Date}, {Fund}, {CompanyName}, {Ticker}, {Cusip}, {Shares}, {MarketValueUSD}, {Weight})";
    }
}
