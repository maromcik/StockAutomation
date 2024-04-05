namespace StockAutomationCore.Diff;

public class HoldingsDiffLine
{
    public HoldingsSnapshotLine Old { get; }
    public HoldingsSnapshotLine New { get; }

    public string CompanyName => New.CompanyName;
    public string Ticker => New.Ticker;

    public int QuantityDiff => New.Quantity - Old.Quantity;

    public HoldingsDiffLine(HoldingsSnapshotLine? oldSnapshot, HoldingsSnapshotLine? newSnapshot)
    {
        if (oldSnapshot.HasValue && newSnapshot.HasValue)
        {
            Old = oldSnapshot.Value;
            New = newSnapshot.Value;
        }
        else if (oldSnapshot.HasValue)
        {
            Old = oldSnapshot.Value;
            New = HoldingsSnapshotLine.DefaultFrom(Old);
        }
        else if (newSnapshot.HasValue)
        {
            New = newSnapshot.Value;
            Old = HoldingsSnapshotLine.DefaultFrom(New);
        }
        else
        {
            throw new InvalidOperationException("Both oldSnapshot and newSnapshot cannot be null");
        }
    }

    public string GetFormattedString()
    {
        var changeEmoji = QuantityDiff > 0 ? "\ud83d\udd3a" : "\ud83d\udd3b";
        var quantityChange = Old.Quantity == 0 ? "" : $"({changeEmoji}{QuantityDiff / Old.Quantity:P})";
        return $"{CompanyName}, {Ticker}, {New.Quantity} , {New.Weight:P}";
    }

}