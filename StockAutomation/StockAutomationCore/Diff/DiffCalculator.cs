using StockAutomationCore.Parser;

namespace StockAutomationCore.Diff;

public static class DiffCalculator
{
    public static string GetDiffText(string pathOld, string pathNew, string newLine = "\r\n")
    {
        var old = HoldingSnapshotLineParser.ParseLines(pathOld);
        var @new = HoldingSnapshotLineParser.ParseLines(pathNew);
        var hd = new HoldingsDiff(old, @new);
        return hd.ToText(newLine);
    }
}