using BusinessLayer.Errors;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services;

public interface ISnapshotService
{
    public Task<IEnumerable<HoldingSnapshot>> GetSnapshotsAsync();
    public Task<Result<bool, Error>> DownloadSnapshotAsync();
    public Task<Result<bool, Error>> DeleteSnapshotsAsync(List<int> ids);
    public Task<Result<string, Error>> CompareSnapshotsAsync(int idNew, int idOld);
    public Task<Result<string, Error>> CompareLatestSnapshotsAsync();
}
