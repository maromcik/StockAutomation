namespace StockAutomationCore.Diff;

public readonly struct HoldingsDiffLine
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
            New = HoldingsSnapshotLine.DefaultFrom(oldSnapshot.Value);
        }
        else if (newSnapshot.HasValue)
        {
            New = newSnapshot.Value;
            Old = HoldingsSnapshotLine.DefaultFrom(newSnapshot.Value);
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

        if (Old.Quantity == 0)
        {
            quantityChange = "";
        }
        else
        {
            var changeValue = float.Abs(QuantityDiff) / Old.Quantity;
            quantityChange = $" ({changeEmoji}{changeValue:0.00%})";
        }

        return $"{CompanyName}, {Ticker}, {New.Quantity}{quantityChange}, {New.Weight:0.00%}";
    }

}