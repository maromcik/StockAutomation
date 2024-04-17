namespace DataAccessLayer.Entities;

public class Configuration : BaseEntity
{
    public required string SnapshotDir { get; set; }
    public required string DownloadUrl { get; set; }
}
