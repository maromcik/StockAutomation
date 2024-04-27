namespace DataAccessLayer.Entities;

public class HoldingSnapshot : BaseEntity
{
    public DateTime DownloadedAt { get; set; }
    ICollection<HoldingSnapshotLineEntity> Lines { get; set; }
}
