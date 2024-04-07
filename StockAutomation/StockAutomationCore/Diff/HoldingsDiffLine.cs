using System.Numerics;
using StockAutomationCore.Model;

namespace StockAutomationCore.Diff;

public readonly struct HoldingsDiffLine
{
    public HoldingSnapshotLine Old { get; }
    public HoldingSnapshotLine New { get; }

    public string CompanyName => New.CompanyName;
    public string Ticker => New.Ticker;
    public string Cusip => New.Cusip;


    public BigInteger QuantityDiff => New.Shares - Old.Shares;

    public HoldingsDiffLine(HoldingSnapshotLine? oldSnapshot, HoldingSnapshotLine? newSnapshot)
    {
        if (oldSnapshot.HasValue && newSnapshot.HasValue)
        {
            Old = oldSnapshot.Value;
            New = newSnapshot.Value;
        }
        else if (oldSnapshot.HasValue)
        {
            Old = oldSnapshot.Value;
            New = HoldingSnapshotLine.CreateDefaultFrom(oldSnapshot.Value);
        }
        else if (newSnapshot.HasValue)
        {
            New = newSnapshot.Value;
            Old = HoldingSnapshotLine.CreateDefaultFrom(newSnapshot.Value);
        }
        else
        {
            throw new InvalidOperationException("Both oldSnapshot and newSnapshot cannot be null");
        }
    }

    public string GetFormattedString()
    {
        var changeEmoji = QuantityDiff > 0 ? "\ud83d\udd3a" : "\ud83d\udd3b";
        string quantityChange;

        if (Old.Shares == 0)
        {
            quantityChange = "";
        }
        else
        {
            var changeValue = BigInteger.Abs(QuantityDiff) / Old.Shares;
            quantityChange = $" ({changeEmoji}{changeValue:0.00%})";
        }

        return $"{CompanyName}, {Ticker}, {New.Shares}{quantityChange}, {New.Weight:0.00%}";
    }
}