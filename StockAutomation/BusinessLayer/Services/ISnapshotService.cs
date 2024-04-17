using DataAccessLayer.Entities;

namespace BusinessLayer.Services;

public interface ISnapshotService
{
    public Task<IEnumerable<Snapshot>> GetSnapshotsAsync();
    public Task DownloadSnapshotAsync();
    public Task DeleteSnapshotAsync(int id);
    public Task<string> CompareSnapshotsAsync(int idNew, int idOld);
}
