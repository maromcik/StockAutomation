using DataAccessLayer.Entities;

namespace BusinessLayer.Services;

public interface ISnapshotService
{
    public Task<IEnumerable<Snapshot>> GetSnapshotsAsync();
    public Task DownloadSnapshotAsync();
    public Task DeleteSnapshotsAsync(List<int> ids);
    public Task<string> CompareSnapshotsAsync(int idNew, int idOld);
}
