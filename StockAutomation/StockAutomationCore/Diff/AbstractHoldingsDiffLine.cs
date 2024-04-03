namespace StockAutomationCore.Diff;

public abstract class AbstractHoldingsDiffLine
{
    public abstract string CompanyName { get; }
    public abstract string Ticker { get; }
}