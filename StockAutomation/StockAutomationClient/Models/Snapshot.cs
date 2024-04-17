namespace StockAutomationClient.Models;

public class Snapshot
{
    public int Id { get; set; }
    public required string FilePath { get; set; }
    public DateTime DownloadedAt { get; set; } = DateTime.Now;
}
