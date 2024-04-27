using BusinessLayer.Errors;
using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using StockAutomationCore.Diff;
using StockAutomationCore.DiffFormat;
using StockAutomationCore.Download;
using StockAutomationCore.Files;
using StockAutomationCore.Model;
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

    public async Task<IEnumerable<HoldingSnapshot>> GetSnapshotsAsync()
    {
        return await _context.HoldingSnapshots.OrderByDescending(s => s.DownloadedAt).ToListAsync();
    }

    public async Task<Result<bool, Error>> DownloadSnapshotAsync()
    {
        try
        {
            var fileBytes = await Downloader.DownloadToBytes(_client, DownloadUrl);
            var parsedFile = HoldingSnapshotLineParser.ParseLinesFromBytes(fileBytes);
            var lines = parsedFile.Select(snapshotLine => new HoldingSnapshotLineEntity
                {
                    Date = snapshotLine.Date,
                    Fund = snapshotLine.Fund,
                    CompanyName = snapshotLine.CompanyName,
                    Ticker = snapshotLine.Ticker,
                    Cusip = snapshotLine.Cusip,
                    Shares = snapshotLine.Shares,
                    MarketValueUsd = snapshotLine.MarketValueUsd,
                    Weight = snapshotLine.Weight,
                }
            ).ToList();

            var holdingSnapshot = new HoldingSnapshot
            {
                DownloadedAt = DateTime.Now,
                Lines = lines
            };
            _context.HoldingSnapshots.Add(holdingSnapshot);

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
        var snapshots = await _context.HoldingSnapshots
            .Where(s => ids.Contains(s.Id)).ToListAsync();
        var snapshotLines = await _context.HoldingSnapshotLines
            .Where(s => ids.Contains(s.HoldingSnapshotId)).ToListAsync();
        if (snapshots.Count == 0 || snapshotLines.Count == 0)
        {
            return new Error
            {
                ErrorType = ErrorType.NoSnapshotsFound,
                Message = "Could not delete selected snapshots - not found"
            };
        }
        _context.HoldingSnapshots.RemoveRange(snapshots);
        _context.HoldingSnapshotLines.RemoveRange(snapshotLines);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Result<string, Error>> CompareSnapshotsAsync(int idNew, int idOld)
    {
        var newSnapshot = _context.HoldingSnapshotLines
            .Where(s => s.HoldingSnapshotId == idNew);
        var oldSnapshot = _context.HoldingSnapshotLines
            .Where(s => s.HoldingSnapshotId == idOld);

        if (oldSnapshot.Count() == 0)
        {
            return new Error
            {
                ErrorType = ErrorType.SnapshotNotFound,
                Message = "Selected old snapshot could not be found"
            };
        }

        if (newSnapshot.Count() == 0)
        {
            return new Error
            {
                ErrorType = ErrorType.SnapshotNotFound,
                Message = "Selected new snapshot could not be found"
            };
        }

        try
        {
            var newSnapshotStruct = newSnapshot.Select(l => HoldingSnapshotLine.Create(
                l.Date, l.Fund, l.CompanyName, l.Ticker, l.Cusip, l.Shares, l.MarketValueUsd, l.Weight));
            var oldSnapshotStruct = oldSnapshot.Select(l => HoldingSnapshotLine.Create(
                l.Date, l.Fund, l.CompanyName, l.Ticker, l.Cusip, l.Shares, l.MarketValueUsd, l.Weight));
            // TODO handle various formats???
            var diff = new HoldingsDiff(oldSnapshotStruct, newSnapshotStruct);
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
