namespace StockAutomationCore.Diff;

public class HoldingsDiffLineOld(HoldingsSnapshotLine oldSnapshot) : AbstractHoldingsDiffLine
{
    public HoldingsSnapshotLine Old { get; } = oldSnapshot;

    public override string CompanyName => Old.CompanyName;
    public override string Ticker => Old.Ticker;
}