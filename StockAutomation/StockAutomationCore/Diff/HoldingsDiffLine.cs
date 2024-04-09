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

    public string ChevronUp => "<img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAYAAABytg0kAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAGElEQVR4nGNwOhSqBcIMjgfD3Rz2hbkDADh8BkHeKoTKAAAAAElFTkSuQmCC'>";

    public string ChevronDown => "<img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAYAAAAeP4ixAAAACXBIWXMAAAsTAAALEwEAmpwYAAACGUlEQVR4nO3YXU8aQRQG4DFeNlZsbetXWxP/RYM5o2LjVe3NnkPBX+itiUkD5IwCplq/KK1V/PwnmNm0sCogzu4AG+dN5nazT3bmnc0RwsXFxcXFxcXleSVZzIyBwrxUVF3Me9MijgFeTQDTrlRU9xdjLXYY4NWEZNprIAKYVCk9JeKynaTCnw8Q/xYwng08JumfCdpvh4gFJrntvQHGymOIBkbR6fxWdlIMGkIy/eoWMZCY5fLaWxNEc+FfYG+i/whFVXPEAGAW8+l3oPB3eESjACrA38ZjjQicmeOeYfQWAKY/USOCmKXc19fWEZLxxBZCNteRNQxwdkYqOu8Bov4f83nHexUpYn6L3oPCC8MXqhqfJ6Y9/d/WfwTjid6OYWoamA70r08oRKrofTBF3G8gjTH9MqBo3xjjIxgvTfd3qxr1MaaNx7Sz8iP78kmIhULmo2S6stE4/h0UAvOp9GXEPoLxsJva1BjTGgfG8qMYYJqVCq9tIqK4k0BRqT2mLobMv4RZTeq7KUQjbrZ9sN6DT//UtBum6/16NyuVo7YPTeW80ZZDAxu1GAbDWNNf02wCEkUdRlD3wHjW9VjJx3SehJSjRAQxssM5NRpa+AO2FhORzo1hDwOKTo0nLy3GO0WbiDt3WeAaCIW4MyFRVJQKN5a/r70QPcpCgeZA4Y3+Z9MXqIhzvHVvWN9v/X4PFxcXFxcXEZfcAg05iUIAeTgAAAAAAElFTkSuQmCC'>";

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
        var changeEmoji = QuantityDiff > 0 ? "\ud83d\udcc8" : "\ud83d\udcc9";
        string quantityChange;

        if (Old.Shares == 0)
        {
            quantityChange = "";
        }
        else
        {
            var changeValue = (decimal) BigInteger.Abs(QuantityDiff) / (decimal) Old.Shares;
            quantityChange = $" ({changeEmoji}{changeValue:0.00%})";
        }

        return $"{CompanyName}, {Ticker}, {New.Shares}{quantityChange}, {New.Weight:0.00%}";
    }
}