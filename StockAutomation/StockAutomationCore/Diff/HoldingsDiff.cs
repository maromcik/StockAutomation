namespace StockAutomationCore.Diff;

public class HoldingsDiff
{
    private Dictionary<string, AbstractHoldingsDiffLine> _holdingsDiffLines;

    public HoldingsDiff(IEnumerable<HoldingsSnapshotLine> oldHoldings, IEnumerable<HoldingsSnapshotLine> newHoldings)
    {
        _holdingsDiffLines = CalculateDiff(oldHoldings, newHoldings);
    }

    public HoldingsDiff(ICollection<AbstractHoldingsDiffLine> holdingsDiffs)
    {
        ArgumentNullException.ThrowIfNull(holdingsDiffs);

        _holdingsDiffLines = holdingsDiffs.ToDictionary(h => h.Ticker);
    }

    private static Dictionary<string, AbstractHoldingsDiffLine> CalculateDiff(IEnumerable<HoldingsSnapshotLine> oldHoldings, IEnumerable<HoldingsSnapshotLine> newHoldings)
    {
        var oldd = oldHoldings.ToDictionary(hl => hl.Ticker);
        var newd = newHoldings.ToDictionary(hl => hl.Ticker);
        var tickers = oldd.Keys.Union(newd.Keys);
        return tickers.ToDictionary(
            t => t,
            t => GetDiffLine(oldd.GetValueOrDefault(t), newd.GetValueOrDefault(t))
        );
    }

    private static AbstractHoldingsDiffLine GetDiffLine(HoldingsSnapshotLine? old, HoldingsSnapshotLine? @new)
    {
        if (old != null && @new != null)
        {
            return new HoldingsDiffLine(old.Value, @new.Value);
        }
        if (old != null)
        {
            return new HoldingsDiffLineOld(old.Value);
        }
        if (@new != null)
        {
            return new HoldingsDiffLineNew(@new.Value);
        }
        throw new InvalidOperationException("Both old and new cannot be null");
    }

    public string ToText()
    {
        throw new NotImplementedException("ToText is not yet implemented");
    }
}