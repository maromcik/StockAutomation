using BusinessLayer.Errors;
using DataAccessLayer.Entities;
using StockAutomationCore.Diff;

namespace BusinessLayer.Services;

public interface ISnapshotService
{
    public Task<IEnumerable<HoldingSnapshot>> GetSnapshotsAsync();
    public Task<Result<bool, Error>> DownloadSnapshotAsync();
    public Task<Result<bool, Error>> DeleteSnapshotsAsync(List<int> ids);

    public Task<Result<HoldingsDiff, Error>> CompareSnapshotsAsync(int idNew, int idOld);

    public Task<Result<HoldingsDiff, Error>> CompareLatestSnapshotsAsync();
    public Task<(string body, string attachment)> FormatDiff(HoldingsDiff diff, OutputFormat emailBodyFormat);
}
