using System.Text;
using StockAutomationCore.Model;
using StockAutomationCore.Parser;

namespace StockAutomationCore.Diff;

public class HoldingsDiff
{
    private readonly Dictionary<string, HoldingsDiffLine> _holdingsDiffLines;

    public HoldingsDiff(IEnumerable<HoldingSnapshotLine> oldHoldings, IEnumerable<HoldingSnapshotLine> newHoldings)
    {
        _holdingsDiffLines = CalculateDiff(oldHoldings, newHoldings);
    }

    public HoldingsDiff(ICollection<HoldingsDiffLine> holdingsDiffs)
    {
        ArgumentNullException.ThrowIfNull(holdingsDiffs);

        _holdingsDiffLines = holdingsDiffs.ToDictionary(h => h.Cusip);
    }

    private static Dictionary<string, HoldingsDiffLine> CalculateDiff(IEnumerable<HoldingSnapshotLine> oldHoldings,
        IEnumerable<HoldingSnapshotLine> newHoldings)
    {
        var oldd = oldHoldings.ToDictionary(
            hl => hl.Cusip,
            hl => new HoldingSnapshotLine?(hl)
        );
        var newd = newHoldings.ToDictionary(
            hl => hl.Cusip,
            hl => new HoldingSnapshotLine?(hl)
        );
        var cusips = oldd.Keys.Union(newd.Keys);
        return cusips.ToDictionary(
            t => t,
            t => new HoldingsDiffLine(oldd.GetValueOrDefault(t), newd.GetValueOrDefault(t))
        );
    }

    private static StringBuilder? GetTextSection(string newLine, string title, List<string> lines)
    {
        if (lines.Count == 0) return null;
        var result = new StringBuilder();
        result.Append(title);
        result.Append(newLine);
        result.AppendJoin(newLine, lines);
        return result;
    }

    public string ToText(string newLine = "\r\n")
    {
        var newPositions = _holdingsDiffLines.Values
            .Where(hdl => hdl.Old.Shares == 0)
            .OrderBy(hdl => hdl.CompanyName)
            .Select(hdl => hdl.GetFormattedString())
            .ToList();
        var increasedPositions = _holdingsDiffLines.Values
            .Where(hdl => hdl.Old.Shares > 0 && hdl.QuantityDiff > 0)
            .OrderBy(hdl => hdl.CompanyName)
            .Select(hdl => hdl.GetFormattedString())
            .ToList();
        var reducedPositions = _holdingsDiffLines.Values
            .Where(hdl => hdl.QuantityDiff < 0)
            .OrderBy(hdl => hdl.CompanyName)
            .Select(hdl => hdl.GetFormattedString())
            .ToList();

        if (newPositions.Count == 0 && increasedPositions.Count == 0 && reducedPositions.Count == 0)
        {
            return "No changes in the index";
        }
        List<StringBuilder?> sections =
        [
            GetTextSection(newLine, "New positions:", newPositions),
            GetTextSection(newLine, "Increased positions:", increasedPositions),
            GetTextSection(newLine, "Reduced positions:", reducedPositions)
        ];
        var result = new StringBuilder();
        result.AppendJoin(newLine + newLine, sections.Where(s => s != null));
        return result.ToString();
    }

    public override string ToString()
    {
        return ToText(Environment.NewLine);
    }
}