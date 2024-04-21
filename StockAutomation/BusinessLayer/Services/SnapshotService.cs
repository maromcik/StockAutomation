using BusinessLayer.Errors;
using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using StockAutomationCore.Diff;
using StockAutomationCore.DiffFormat;
using StockAutomationCore.Download;
using StockAutomationCore.Files;
using StockAutomationCore.Parser;

namespace BusinessLayer.Services;

public class SnapshotService : ISnapshotService
{
    private readonly StockAutomationDbContext _context;
    private readonly HttpClient _client;
    private string DownloadUrl { get; set; }
    private string SnapshotDir { get; set; }

    public SnapshotService(StockAutomationDbContext context, HttpClient client)
    {
        _context = context;
        _client = client;
        SnapshotDir = Directory.GetCurrentDirectory() + "/../" + "/snapshots";
        FileUtils.CreateSnapshotDir(SnapshotDir);
        var url = context.Configurations.FirstOrDefault()?.DownloadUrl;
        if (url is null)
        {
            DownloadUrl = client.BaseAddress?.ToString() ?? string.Empty;
        }
        else
        {
            DownloadUrl = url;
        }

    }

    public async Task<IEnumerable<Snapshot>> GetSnapshotsAsync()
    {
        return await _context.Snapshots.OrderByDescending(s => s.DownloadedAt).ToListAsync();
    }

    public async Task<Result<bool, Error>> DownloadSnapshotAsync()
    {
        try
        {
            var filename = await Downloader.DownloadToFile(_client, DownloadUrl, SnapshotDir);
            var file = new Snapshot
            {
                FileName = filename
            };
            _context.Snapshots.Add(file);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return new Error
            {
                ErrorType = ErrorType.DownloadError,
                Message = e.Message
            };
        }
    }

    public async Task<Result<bool, Error>> DeleteSnapshotsAsync(List<int> ids)
    {
        var snapshots = await _context.Snapshots.Where(s => ids.Contains(s.Id)).ToListAsync();
        if (snapshots.Count == 0)
        {
            return new Error
            {
                ErrorType = ErrorType.NoSnapshotsFound,
                Message = "Could not delete selected snapshots - not found"
            };
        }
        _context.Snapshots.RemoveRange(snapshots);
        FileUtils.DeleteFiles(snapshots.Select(s => GetFullPath(s.FileName)));
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Result<string, Error>> CompareSnapshotsAsync(int idNew, int idOld)
    {
        var newSnapshot = await _context.Snapshots.FirstOrDefaultAsync(s => s.Id == idNew);
        var oldSnapshot = await _context.Snapshots.FirstOrDefaultAsync(s => s.Id == idOld);

        if (oldSnapshot is null)
        {
            return new Error
            {
                ErrorType = ErrorType.SnapshotNotFound,
                Message = "Selected old snapshot could not be found"
            };
        }

        if (newSnapshot is null)
        {
            return new Error
            {
                ErrorType = ErrorType.SnapshotNotFound,
                Message = "Selected new snapshot could not be found"
            };
        }

        try
        {
            var parsedOldFile = HoldingSnapshotLineParser.ParseLines(GetFullPath(oldSnapshot.FileName));
            var parsedNewFile = HoldingSnapshotLineParser.ParseLines(GetFullPath(newSnapshot.FileName));
            // TODO handle various formats
            var diff = new HoldingsDiff(parsedOldFile, parsedNewFile);
            return TextDiffFormatter.Format(diff);
        }
        catch (IOException)
        {
            return new Error
            {
                ErrorType = ErrorType.FileNotFound,
                Message = "File could not be found."
            };
        }
    }

    private string GetFullPath(string filename)
    {
        return $"{SnapshotDir}/{filename}";
    }
}
