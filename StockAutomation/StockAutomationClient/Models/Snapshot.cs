namespace StockAutomationClient.Models;

public class Snapshot
{
    public int Id { get; set; }
    public required string FileName { get; set; }
    public DateTime DownloadedAt { get; set; } = DateTime.Now;
}