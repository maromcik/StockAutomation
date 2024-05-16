using System.Numerics;
using System.Text;
using StockAutomationCore.Diff;

namespace StockAutomationCore.DiffFormat;

public static class TextDiffFormatter
{
    public static string FormatText(this HoldingsDiff diff, string newLine = "\r\n")
    {
        var newPositions = diff.HoldingsDiffLines.Values
            .Where(hdl => hdl.Old.Shares == 0)
            .OrderBy(hdl => hdl.CompanyName)
            .Select(GetFormattedLine)
            .ToList();
        var increasedPositions = diff.HoldingsDiffLines.Values
            .Where(hdl => hdl.Old.Shares > 0 && hdl.QuantityDiff > 0)
            .OrderBy(hdl => hdl.CompanyName)
            .Select(GetFormattedLine)
            .ToList();
        var reducedPositions = diff.HoldingsDiffLines.Values
            .Where(hdl => hdl.QuantityDiff < 0)
            .OrderBy(hdl => hdl.CompanyName)
            .Select(GetFormattedLine)
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

    private static StringBuilder? GetTextSection(string newLine, string title, List<string> lines)
    {
        if (lines.Count == 0) return null;
        var result = new StringBuilder();
        result.Append(title);
        result.Append(newLine);
        result.AppendJoin(newLine, lines);
        return result;
    }

    private static string GetFormattedLine(HoldingsDiffLine diffLine)
    {
        var changeEmoji = diffLine.QuantityDiff > 0 ? "\ud83d\udcc8" : "\ud83d\udcc9";
        string quantityChange;

        if (diffLine.Old.Shares == 0)
        {
            quantityChange = "";
        }
        else
        {
            var changeValue = (decimal)BigInteger.Abs(diffLine.QuantityDiff) / (decimal)diffLine.Old.Shares;
            quantityChange = $" ({changeEmoji}{changeValue:0.00%})";
        }

        return
            $"{diffLine.CompanyName}, {diffLine.Ticker}, {diffLine.New.Shares}{quantityChange}, {diffLine.New.Weight:0.00%}";
    }
}