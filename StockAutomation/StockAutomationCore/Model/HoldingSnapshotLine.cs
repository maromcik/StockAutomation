using System.Numerics;
using System.Text.RegularExpressions;

namespace StockAutomationCore.Model;

public partial class HoldingSnapshotLine
{
    private DateTime _date { get; } // apparently no `Date` type
    private string _fund { get; }
    private string _company { get; }
    private string _ticker { get; }
    private string _cusip { get; }
    private BigInteger _shares { get; }
    private Decimal _marketValueUSD { get; }
    private Decimal _weight { get; }

    private static Regex lineRegex = TheLineRegex();

    private HoldingSnapshotLine(DateTime date, string fund, string company, string ticker, string cusip, BigInteger shares, Decimal marketValueUSD, Decimal weight)
    {
        _date = date;
        _fund = fund;
        _company = company;
        _ticker = ticker;
        _cusip = cusip;
        _shares = shares;
        _marketValueUSD = marketValueUSD;
        _weight = weight;
    }

    public static HoldingSnapshotLine Create(DateTime date, string fund, string company, string ticker, string cusip, BigInteger shares, Decimal marketValueUSD, Decimal weight)
    {
        // todo assertions
        return new HoldingSnapshotLine(date, fund, company, ticker, cusip, shares, marketValueUSD, weight);
    }

    public static HoldingSnapshotLine From(string line)
    {
        // todo parse line
        Match match = lineRegex.Match(line);

        if (match.Success == false)
        {
            throw new ArgumentException("Invalid line format");
        }

        string date = match.Groups[1].Value;
        string fund = match.Groups[2].Value;
        string company = match.Groups[3].Value;
        string ticker = match.Groups[4].Value;
        string cusip = match.Groups[5].Value;
        string shares = match.Groups[6].Value;
        string marketValueUSD = match.Groups[7].Value;
        string weight = match.Groups[8].Value;

        Console.WriteLine($"date: {date}, fund: {fund}, company: {company}, ticker: {ticker}, cusip: {cusip}, shares: {shares}, marketValueUSD: {marketValueUSD}, weight: {weight}");

        throw new NotImplementedException("todo");
    }

    // what kind of witchcraft is this
    [GeneratedRegex(@"^(\d{2}/\d{2}/\d{4}),([^,]+),""([^""]+)"",([^,]+),([^,]+),""([^""]+)"",""\$([^""]+)"",([^%]+)%$")]
    private static partial Regex TheLineRegex();
}
