using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using StockAutomationCore.Model;

namespace StockAutomationCore.Parser;

public static partial class HoldingSnapshotLineParser
{
    private static readonly Regex LineRegex = TheLineRegex();

    /**
        * Parses a line from the holding snapshot file.
        *
        * @param line the line to parse
        * @return a `HoldingSnapshotLine` if the line is valid, otherwise `null`
        */
    public static HoldingSnapshotLine? Parse(string line)
    {
        Match match = LineRegex.Match(line);
        if (match.Success == false) return null;

        string date = match.Groups[1].Value;
        string fund = match.Groups[2].Value;
        string _company_fst = match.Groups[4].Value;
        string company = _company_fst == "" ? match.Groups[5].Value : _company_fst;
        string _ticker_fst = match.Groups[7].Value;
        string ticker = _ticker_fst == "" ? match.Groups[8].Value : _ticker_fst;
        string cusip = match.Groups[9].Value;
        string _shares_fst = match.Groups[11].Value;
        string shares = _shares_fst == "" ? match.Groups[12].Value : _shares_fst;
        string marketValueUSD = match.Groups[13].Value;
        string weight = match.Groups[14].Value;

        if (!DateTime.TryParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime properDate)) return null;
        string properFund = fund; // trivial
        string properCompany = company; // trivial
        string properTicker = ticker; // trivial
        string properCusip = cusip; // trivial
        if (!BigInteger.TryParse(shares.Replace(",", ""), out BigInteger properShares)) return null;
        if (!decimal.TryParse(marketValueUSD, out decimal properMarketValueUSD)) return null;
        if (!decimal.TryParse(weight, out decimal properWeight)) return null;

        return HoldingSnapshotLine.Create(
            properDate, properFund, properCompany, properTicker, properCusip, properShares, properMarketValueUSD,
            properWeight
        );
    }

    [GeneratedRegex(@"^(\d{2}/\d{2}/\d{4}),([^,]+),(""([^,]+)""|([^,]+)),(""([^""]*)""|([^,]*)),([^,]+),(""([^"".]+)""|([^,]+)),""\$(.+)"",([^%]+)%$")]
    private static partial Regex TheLineRegex();

    public static IEnumerable<HoldingSnapshotLine> ParseLines(string filepath)
        => File.ReadLines(filepath)
            .Skip(1) /* ingnore header */
            .SkipLast(1) /* ignore whatever that is */
            .Select(line => Parse(line) ?? throw new Exception($"Could not parse line: {line}"));
}