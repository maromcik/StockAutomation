namespace DataAccessLayer.Entities;

public class Snapshot : BaseEntity
{
    public required string FileName { get; set; }
    public DateTime DownloadedAt { get; set; } = DateTime.Now;
}
