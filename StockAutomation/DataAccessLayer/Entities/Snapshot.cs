namespace DataAccessLayer.Entities;

public class Snapshot
{
    public required string FilePath { get; set; }
    public DateTime DownloadedAt { get; set; } = DateTime.Now;
}
