namespace DataAccessLayer.Entities;

public class Configuration : BaseEntity
{
    public required string SnapshotDir { get; set; }
    public string? DownloadUrl { get; set; }

    public required OutputFormat OutputFormat { get; set; } = OutputFormat.HTML;
}
