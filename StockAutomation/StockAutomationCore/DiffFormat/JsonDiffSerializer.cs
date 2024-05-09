using System.Text.Json;
using StockAutomationCore.Diff;

namespace StockAutomationCore.DiffFormat;

public static class JsonDiffSerializer
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Converters = { new BigIntegerConverter() }
    };

    public static string SerializeJson(this HoldingsDiff diff)
    {
        return JsonSerializer.Serialize(diff.HoldingsDiffLines.Values, Options);
    }
}
