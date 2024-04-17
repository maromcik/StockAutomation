using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using StockAutomationCore.Diff;
using StockAutomationCore.DiffFormat;
using StockAutomationCore.Download;
using StockAutomationCore.Parser;

namespace BusinessLayer.Services;

public class SnapshotService : ISnapshotService
{
    private readonly StockAutomationDbContext _context;
    private readonly DownloadController _downloadController;

    public SnapshotService(StockAutomationDbContext context)
    {
        _context = context;
        var url = context.Configurations.FirstOrDefault()?.DownloadUrl;
        _downloadController = url is null ? new DownloadController() : new DownloadController(url);
    }

    public async Task<IEnumerable<Snapshot>> GetSnapshotsAsync()
    {
        return await _context.Snapshots.ToListAsync();
    }

    public async Task DownloadSnapshotAsync()
    {
        var filename = await _downloadController.DownloadToFile();
        var file = new Snapshot
        {
            FilePath = filename,
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
