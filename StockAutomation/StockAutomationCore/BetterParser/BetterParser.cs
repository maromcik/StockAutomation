using System.Globalization;
using System.Numerics;
using Microsoft.VisualBasic.FileIO;
using StockAutomationCore.Model;

namespace StockAutomationCore.BetterParser
{
    public class BetterParser
    {
        public static IEnumerable<HoldingSnapshotLine> ParseFile(string path)
        {
            using var lineParser = new TextFieldParser(new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                HasFieldsEnclosedInQuotes = true
            };
            lineParser.SetDelimiters(",");

            // List<HoldingSnapshotLine> lines = [];
            var snapshots = new List<HoldingSnapshotLine>();

            lineParser.ReadLine(); // skip header
            while (!lineParser.EndOfData)
            {
                var fields = lineParser.ReadFields() ?? throw new InvalidOperationException("why tf would this return null");

                if (!DateTime.TryParseExact(fields[0], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date)) yield break;
                string fund = fields[1];
                string companyName = fields[2].Replace("\"", "");
                string ticker = fields[3].Replace("\"", "");
                string cusip = fields[4];
                BigInteger shares = BigInteger.Parse(fields[5].Replace(",", ""));
                decimal marketValueUSD = decimal.Parse(fields[6].Replace("$", "").Replace(",", ""));
                decimal weight = decimal.Parse(fields[7].Replace("%", ""));

                var line = HoldingSnapshotLine.Create(date, fund, companyName, ticker, cusip, shares, marketValueUSD, weight);

                Console.WriteLine(line);

                yield return line;
            }
        }
    }
}
