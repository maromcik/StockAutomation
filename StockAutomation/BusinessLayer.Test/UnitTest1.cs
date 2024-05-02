using System.Transactions;
using BusinessLayer.Models;
using BusinessLayer.Services;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Test;

[TestFixture]
[NonParallelizable]
public class Tests
{
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
}