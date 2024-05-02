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
}
