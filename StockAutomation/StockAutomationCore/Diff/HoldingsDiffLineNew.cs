namespace StockAutomationCore.Diff;

public class HoldingsDiffLineNew(HoldingsSnapshotLine newSnapshot) : AbstractHoldingsDiffLine
{
    public HoldingsSnapshotLine New { get; } = newSnapshot;

    public override string CompanyName => New.CompanyName;
    public override string Ticker => New.Ticker;
}