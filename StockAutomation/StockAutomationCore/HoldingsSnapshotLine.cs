using StockAutomationCore.Error;

namespace StockAutomationCore;

public readonly struct HoldingsSnapshotLine
{
    public string CompanyName { get; }
    public string Ticker { get; }
    public int Quantity { get; }
    public float Weight { get; }

    public static Result<HoldingsSnapshotLine, Error.Error> New(string companyName, string ticker, int quantity, float weight)
    {
        if (int.IsNegative(quantity))
        {
            return Error.Error.EmptyString; // TODO: Create a new error type for negative quantity
        }

        if (weight is < 0 or > 1)
        {
            return Error.Error.EmptyString; // TODO: Create a new error type for negative weight
        }

        return new HoldingsSnapshotLine(companyName, ticker, quantity, weight);
    }

    public static HoldingsSnapshotLine DefaultFrom(HoldingsSnapshotLine other)
    {
        return new HoldingsSnapshotLine(other.CompanyName, other.Ticker);
    }

    private HoldingsSnapshotLine(string companyName, string ticker, int quantity, float weight)
    {
        CompanyName = companyName;
        Ticker = ticker;
        Quantity = quantity;
        Weight = weight;
    }

    private HoldingsSnapshotLine(string companyName, string ticker)
    {
        CompanyName = companyName;
        Ticker = ticker;
        Quantity = 0;
        Weight = 0;
    }
}