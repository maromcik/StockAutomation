using DataAccessLayer.Entities;
using StockAutomationCore.Diff;
using StockAutomationCore.DiffFormat;

namespace BusinessLayer.Models;

public static class OutputFormatting
{
    public static string FormatDiff(this OutputFormat outputFormat, HoldingsDiff diff)
    {
        return outputFormat switch
        {
            OutputFormat.JSON => diff.SerializeJson(),
            OutputFormat.HTML => diff.FormatHtml(),
            _ => diff.FormatText()
        };
    }

    public static string GetFileExtension(this OutputFormat outputFormat)
    {
        return outputFormat switch
        {
            OutputFormat.JSON => "json",
            OutputFormat.HTML => "html",
            _ => "txt"
        };
    }
}