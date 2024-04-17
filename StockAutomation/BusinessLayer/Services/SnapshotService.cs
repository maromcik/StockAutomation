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
            FilePath = $"{FileUtils.SnapshotDir}/{filename}",
        };
        _context.Snapshots.Add(file);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSnapshotAsync(int id)
    {
        var snapshot = await _context.Snapshots.FirstOrDefaultAsync(s => s.Id == id);
        if (snapshot is null)
        {
            // TODO handle snapshot not found
            return;
        }
        _context.Snapshots.Remove(snapshot);
        await _context.SaveChangesAsync();
    }

    public async Task<string> CompareSnapshotsAsync(int idNew, int idOld)
    {
        var newFileName = await _context.Snapshots.FirstOrDefaultAsync(s => s.Id == idNew);
        var oldFileName = await _context.Snapshots.FirstOrDefaultAsync(s => s.Id == idOld);

        if (oldFileName is null)
        {
            // TODO handle oldfile not found
            return "";
        }

        if (newFileName is null)
        {
            // TODO handle newfile not found
            return "";
        }

        var parsedOldFile = HoldingSnapshotLineParser.ParseLines(oldFileName.FilePath);
        var parsedNewFile = HoldingSnapshotLineParser.ParseLines(newFileName.FilePath);

        // TODO handle various formats
        var diff = new HoldingsDiff(parsedOldFile, parsedNewFile);
        return TextDiffFormatter.Format(diff);
    }
}
