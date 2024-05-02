// using System.Transactions;
using BusinessLayer.Services;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using StockAutomationCore.Download;

namespace BusinessLayer.Test;


public class SnapshotServiceTests
{
    private const string TestSnapshotFile = "../../../test_snapshot.csv"; // no comment

    private class DownloaderMock : IDownloader
    {
        public Task<string> DownloadToFile(HttpClient client, string downloadUrl, string snapshotDir)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> DownloadToBytes(HttpClient _c, string _s)
        {
            // abuse the opportunity to specify the filepath in the downloadUrl
            if (!File.Exists(TestSnapshotFile))
            {
                throw new FileNotFoundException();
            }

            return Task.FromResult(File.ReadAllBytes(TestSnapshotFile));
        }
    }

    private DbContextOptions _options;
    // private TransactionScope _transaction;

    [SetUp]
    public void Setup()
    {
        // todo either fix or remove the commented out code
        // me or the framework is fckin retarded and cannot isolate transactions on a single db properly
        var uniqueDbName = Guid.NewGuid().ToString();

        _options = new DbContextOptionsBuilder().UseInMemoryDatabase(uniqueDbName).Options;

        // _transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    }

    [TearDown]
    public void TearDown()
    {
        // _transaction.Dispose();
    }

    [Test]
    public async Task PlsWork()
    {
        // Arrange

        var context = new StockAutomationDbContext(_options);

        var service = new SnapshotService<DownloaderMock>(context, new HttpClient());

        Assert.That((await service.DownloadSnapshotAsync()).IsOk);
    }

    [Test]
    public async Task GetSnapshotsAsync_DownloadedSnapshot_ReturnsThatSnapshot()
    {
        // Arrange
        var context = new StockAutomationDbContext(_options);
        var service = new SnapshotService<DownloaderMock>(context, new HttpClient());
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
        var context = new StockAutomationDbContext(_options);
        var service = new SnapshotService<DownloaderMock>(context, new HttpClient());
        await service.DownloadSnapshotAsync();
        var snapshots = await service.GetSnapshotsAsync();
        var snapshot = snapshots.First();

        // Act
        await service.DeleteSnapshotsAsync([snapshot.Id]);
        var snapshotsAfter = await service.GetSnapshotsAsync();

        // Assert
        Assert.That(snapshotsAfter.Count(), Is.EqualTo(0));
    }

    // todo inconsistant behavior of `DeleteSnapshotsAsync`
    //  - would expect either both of DeleteSnapshotsAsync_TargetOneExistingAndOneNonExistingEntry_DeletesExistingAndReturnsOk
    //    and DeleteSnapshotsAsync_TargetSingleNonExistingEntry_ReturnsOk to fail or both of them to succeed

    [Test]
    public async Task DeleteSnapshotsAsync_TargetOneExistingAndOneNonExistingEntry_DeletesExistingAndReturnsOk()
    {
        // Arrange

        var context = new StockAutomationDbContext(_options);
        var service = new SnapshotService<DownloaderMock>(context, new HttpClient());
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
            Assert.That(result.IsOk);
            Assert.That(snapshotsAfter.Count(), Is.EqualTo(0));
        });
    }

    // todo either make pass & uncomment or delete
    // [Test]
    // public async Task DeleteSnapshotsAsync_TargetSingleNonExistingEntry_ReturnsOk()
    // {
    //     // Arrange
    //     var context = new StockAutomationDbContext(_options);
    //     var service = new SnapshotService<DownloaderMock>(context, new HttpClient());
    //     var snapshots = await service.GetSnapshotsAsync();
    //     var nonexistentId = 420;

    //     // Act
    //     var result = await service.DeleteSnapshotsAsync([nonexistentId]);
    //     var snapshotsAfter = await service.GetSnapshotsAsync();

    //     // Assert
    //     Assert.Multiple(() =>
    //     {
    //         Assert.That(result.IsOk);
    //         Assert.That(snapshotsAfter.Count(), Is.EqualTo(0));
    //     });
    // }

    [Test]
    public async Task DeleteSnapshotsAsync_TargetOneOfExistingEntries_OthersUnaffected()
    {
        // Arrange

        var context = new StockAutomationDbContext(_options);
        var service = new SnapshotService<DownloaderMock>(context, new HttpClient());

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
        var context = new StockAutomationDbContext(_options);
        var service = new SnapshotService<DownloaderMock>(context, new HttpClient());
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
