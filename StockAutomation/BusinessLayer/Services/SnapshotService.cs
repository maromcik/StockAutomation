using BusinessLayer.Errors;
using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using StockAutomationCore.Diff;
using StockAutomationCore.DiffFormat;
using StockAutomationCore.Download;
using StockAutomationCore.Model;
using StockAutomationCore.Parser;

namespace BusinessLayer.Services;

public class SnapshotService(StockAutomationDbContext context, HttpClient client) : ISnapshotService
{
    public async Task<IEnumerable<HoldingSnapshot>> GetSnapshotsAsync()
    {
        return await context.HoldingSnapshots.OrderByDescending(s => s.DownloadedAt).ToListAsync();
    }

    public async Task<Result<bool, Error>> DownloadSnapshotAsync()
    {
        try
        {
            var fileBytes = await Downloader.DownloadToBytes(client);
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
            context.HoldingSnapshots.Add(holdingSnapshot);

            await context.SaveChangesAsync();
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
        var snapshots = await context.HoldingSnapshots
            .Where(s => ids.Contains(s.Id)).ToListAsync();
        if (snapshots.Count == 0)
        {
            return new Error
            {
                ErrorType = ErrorType.SnapshotsNotFound,
                Message = "Could not delete selected snapshots - not found"
            };
        }

        context.HoldingSnapshots.RemoveRange(snapshots);

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Result<string, Error>> CompareSnapshotsAsync(int idNew, int idOld)
    {
        var newSnapshot = await context
            .HoldingSnapshots
            .Include(s => s.Lines)
            .FirstOrDefaultAsync(s => s.Id == idNew);
        var oldSnapshot = await context
            .HoldingSnapshots
            .Include(s => s.Lines)
            .FirstOrDefaultAsync(s => s.Id == idOld);
        return CompareSnapshotLinesAsync(newSnapshot, oldSnapshot);
    }

    public async Task<Result<string, Error>> CompareLatestSnapshotsAsync()
    {
        var latestSnapshots = await context
            .HoldingSnapshots
            .Include(s => s.Lines)
            .OrderByDescending(s => s.DownloadedAt)
            .Take(2)
            .ToListAsync();

        if (latestSnapshots.Count != 2)
        {
            return new Error
            {
                ErrorType = ErrorType.SnapshotsNotFound,
                Message = "At least two or more snapshots must be downloaded."
            };
        }

        return CompareSnapshotLinesAsync(latestSnapshots[0], latestSnapshots[1]);
    }

    private Result<string, Error> CompareSnapshotLinesAsync(HoldingSnapshot? newSnapshot, HoldingSnapshot? oldSnapshot)
    {
        if (newSnapshot == null)
        {
            return new Error
            {
                ErrorType = ErrorType.SnapshotNotFound,
                Message = "Selected new snapshot could not be found"
            };
        }

        if (oldSnapshot == null)
        {
            return new Error
            {
                ErrorType = ErrorType.SnapshotNotFound,
                Message = "Selected old snapshot could not be found"
            };
        }

        if (oldSnapshot.Lines.Count == 0)
        {
            return new Error
            {
                ErrorType = ErrorType.SnapshotEmpty,
                Message = "Selected old snapshot is empty"
            };
        }

        if (newSnapshot.Lines.Count == 0)
        {
            return new Error
            {
                ErrorType = ErrorType.SnapshotEmpty,
                Message = "Selected new snapshot is empty"
            };
        }

        var newSnapshotStruct = newSnapshot.Lines.Select(l => HoldingSnapshotLine.Create(
            l.Date, l.Fund, l.CompanyName, l.Ticker, l.Cusip, l.Shares, l.MarketValueUsd, l.Weight));
        var oldSnapshotStruct = oldSnapshot.Lines.Select(l => HoldingSnapshotLine.Create(
            l.Date, l.Fund, l.CompanyName, l.Ticker, l.Cusip, l.Shares, l.MarketValueUsd, l.Weight));
        // TODO handle various formats???
        var diff = new HoldingsDiff(oldSnapshotStruct, newSnapshotStruct);
        return TextDiffFormatter.Format(diff);
    }
}
