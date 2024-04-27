namespace DataAccessLayer.Entities;

public class HoldingSnapshot : BaseEntity
{
    public DateTime DownloadedAt { get; set; }
    public ICollection<HoldingSnapshotLineEntity> Lines { get; set; } = new List<HoldingSnapshotLineEntity>();
}
