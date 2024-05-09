using StockAutomationCore.Diff;

namespace StockAutomationCore.DiffFormat;

public static class JsonDiffSerializer
{
    public static string SerializeJson(this HoldingsDiff diff)
    {
        return System.Text.Json.JsonSerializer.Serialize(diff.HoldingsDiffLines.Values);
    }
}
