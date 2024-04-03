namespace StockAutomationCore.Diff;

public class HoldingsDiffLine(HoldingsSnapshotLine oldSnapshot, HoldingsSnapshotLine newSnapshot)
    : AbstractHoldingsDiffLine
{
    public HoldingsSnapshotLine Old { get; } = oldSnapshot;
    public HoldingsSnapshotLine New { get; } = newSnapshot;

    public override string CompanyName => New.CompanyName;
    public override string Ticker => New.Ticker;

    public int QuantityDiff => New.Quantity - Old.Quantity;
}