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
        SnapshotDir = Directory.GetCurrentDirectory() + "/snapshots";
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
        return await _context.Snapshots.ToListAsync();
    }

    public async Task DownloadSnapshotAsync()
    {
        var filename = await Downloader.DownloadToFile(_client, DownloadUrl, SnapshotDir);
        var file = new Snapshot
        {
            FilePath = filename
        };
        _context.Snapshots.Add(file);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSnapshotsAsync(List<int> ids)
    {
        var snapshots = await _context.Snapshots.Where(s => ids.Contains(s.Id)).ToListAsync();
        _context.Snapshots.RemoveRange(snapshots);
        FileUtils.DeleteFiles(snapshots.Select(s => GetFullPath(s.FilePath)));
        await _context.SaveChangesAsync();
    }

    public async Task<string> CompareSnapshotsAsync(int idNew, int idOld)
    {
        var newSnapshot = await _context.Snapshots.FirstOrDefaultAsync(s => s.Id == idNew);
        var oldSnapshot = await _context.Snapshots.FirstOrDefaultAsync(s => s.Id == idOld);

        if (oldSnapshot is null)
        {
            // TODO handle oldfile not found
            return "";
        }

        if (newSnapshot is null)
        {
            // TODO handle newfile not found
            return "";
        }

        var parsedOldFile = HoldingSnapshotLineParser.ParseLines(GetFullPath(oldSnapshot.FilePath));
        var parsedNewFile = HoldingSnapshotLineParser.ParseLines(GetFullPath(newSnapshot.FilePath));

        // TODO handle various formats
        var diff = new HoldingsDiff(parsedOldFile, parsedNewFile);
        return TextDiffFormatter.Format(diff);
    }

    private string GetFullPath(string filename)
    {
        return $"{SnapshotDir}/{filename}";
    }
}
