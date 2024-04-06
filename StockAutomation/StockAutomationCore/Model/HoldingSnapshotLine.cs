using System.Numerics;

namespace StockAutomationCore.Model;

public partial class HoldingSnapshotLine
{
    public DateTime Date { get; } // apparently no `Date` type
    public string Fund { get; }
    public string Company { get; }
    public string Ticker { get; }
    public string Cusip { get; }
    public BigInteger Shares { get; }
    public decimal MarketValueUSD { get; }
    public decimal Weight { get; }

    private HoldingSnapshotLine(DateTime date, string fund, string company, string ticker, string cusip, BigInteger shares, decimal marketValueUSD, decimal weight)
    {
        Date = date;
        Fund = fund;
        Company = company;
        Ticker = ticker;
        Cusip = cusip;
        Shares = shares;
        MarketValueUSD = marketValueUSD;
        Weight = weight;
    }

    public static HoldingSnapshotLine Create(DateTime date, string fund, string company, string ticker, string cusip, BigInteger shares, decimal marketValueUSD, decimal weight)
    {
        // todo assertions
        return new HoldingSnapshotLine(date, fund, company, ticker, cusip, shares, marketValueUSD, weight);
    }

    public override string ToString()
    {
        return $"HoldingSnapshotLine({Date}, {Fund}, {Company}, {Ticker}, {Cusip}, {Shares}, {MarketValueUSD}, {Weight})";
    }
}
