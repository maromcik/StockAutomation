// using System.Transactions;
using BusinessLayer.Services;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Test;


public class SnapshotServiceTests
{
    private SnapshotService service;

    [SetUp]
    public void Setup()
    {
        var uniqueDbName = Guid.NewGuid().ToString();

        var options = new DbContextOptionsBuilder().UseInMemoryDatabase(uniqueDbName).Options;

        var context = new StockAutomationDbContext(options);
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "StockAutomationCore/1.0");
        client.BaseAddress = new Uri("https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv");

        service = new SnapshotService(context, client);
    }

    [Test]
    public async Task GetSnapshotsAsync_DownloadedSnapshot_ReturnsThatSnapshot()
    {
        // Arrange
        await service.DownloadSnapshotAsync();

        // Act
        var snapshots = await service.GetSnapshotsAsync();

        // Assert
        Assert.That(snapshots.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task DeleteSnapshotsAsync_TargetSingleExistingEntry_DeletesThatEntry()
    {
        // Arrange
        await service.DownloadSnapshotAsync();
        var snapshots = await service.GetSnapshotsAsync();
        var snapshot = snapshots.First();

        // Act
        await service.DeleteSnapshotsAsync([snapshot.Id]);
        var snapshotsAfter = await service.GetSnapshotsAsync();

        // Assert
        Assert.That(snapshotsAfter.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task DeleteSnapshotsAsync_TargetOneExistingAndOneNonExistingEntry_ReturnsErrorAndDoesNotDelete()
    {
        // Arrange
        await service.DownloadSnapshotAsync();
        var snapshots = await service.GetSnapshotsAsync();
        var snapshot = snapshots.First();

        // Act

        var nonexistentId = 420;
        Assert.That(snapshots.Any(s => s.Id == nonexistentId), Is.False);

        var result = await service.DeleteSnapshotsAsync([snapshot.Id, nonexistentId]);
        var snapshotsAfter = await service.GetSnapshotsAsync();
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(!result.IsOk);
            Assert.That(snapshotsAfter.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task DeleteSnapshotsAsync_TargetSingleNonExistingEntry_ReturnsError()
    {
        // Arrange
        var snapshots = await service.GetSnapshotsAsync();
        var nonexistentId = 420;

        // Act
        var result = await service.DeleteSnapshotsAsync([nonexistentId]);
        var snapshotsAfter = await service.GetSnapshotsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(!result.IsOk);
            Assert.That(snapshotsAfter.Count(), Is.EqualTo(0));
        });
    }

    [Test]
    public async Task DeleteSnapshotsAsync_TargetOneOfExistingEntries_OthersUnaffected()
    {
        // Arrange

        await service.DownloadSnapshotAsync();
        var snapshotsAfterFirstDownload = await service.GetSnapshotsAsync();
        var firstSnaposhot = snapshotsAfterFirstDownload.First();

        await service.DownloadSnapshotAsync();

        // Act

        await service.DeleteSnapshotsAsync([firstSnaposhot.Id]);
        var snapshotsAfter = await service.GetSnapshotsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(snapshotsAfter.Count(), Is.EqualTo(1));
            Assert.That(snapshotsAfter.First().Id, Is.Not.EqualTo(firstSnaposhot.Id));
        });
    }

    [Test]
    public async Task CompareSnapshotsAsync_SameSnapshot_IsOk()
    {
        // Arrange
        await service.DownloadSnapshotAsync();
        var snapshots = await service.GetSnapshotsAsync();
        var snapshot = snapshots.First();

        // Act
        var result = await service.CompareSnapshotsAsync(snapshot.Id, snapshot.Id);

        Console.WriteLine(result.Value);

        // Assert
        Assert.That(result.IsOk);
    }
}
