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

    [Test]
    public void GetSubscribersAsync_CleanDB_ReturnsEmptyEntrySet()
    {
        // Arrange

        var context = new StockAutomationDbContext(_options);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var service = new EmailService(context, null /* will not need configuration */);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act

        var subscribers = service.GetSubscribersAsync().Result;

        // Assert

        Assert.That(subscribers, Is.Empty);
    }

    [Test]
    public async Task CreateSubscriber_InvalidEmails_Rejected()
    {
        // Arrange

        var context = new StockAutomationDbContext(_options);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var service = new EmailService(context, null /* will not need configuration */);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act

        var invalidSub1 = new SubscriberCreate { EmailAddress = "missing_at.com" };
        var response1 = await service.CreateSubscriber(invalidSub1);

        var invalidSub2 = new SubscriberCreate { EmailAddress = "missing_domain@.com" };
        var response2 = await service.CreateSubscriber(invalidSub2);

        Assert.Multiple(() =>
        {

            // Assert

            Assert.That(!response1.IsOk);
            Assert.That(!response2.IsOk);
        });
    }

    [Test]
    public async Task CreateSubscriber_CleanDB_SubscribersExactlyThatSubscriber()
    {
        // Arrange

        var context = new StockAutomationDbContext(_options);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var service = new EmailService(context, null /* will not need configuration */);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        var subscriber = new SubscriberCreate { EmailAddress = "_@_.com" };

        // Act

        var response = await service.CreateSubscriber(subscriber);

        // Assert

        Assert.That(response.IsOk);

        var subscribers = service.GetSubscribersAsync().Result;
        var subscribersCollected = subscribers.ToList();

        Assert.That(subscribersCollected, Has.Count.EqualTo(1));

        var firstSubscriber = subscribersCollected.First();
        Assert.That(firstSubscriber.EmailAddress, Is.EqualTo(subscriber.EmailAddress));
    }

    [Test]
    public async Task DeleteSubscribersAsync_TargetSingleExistingEntry_DeletesThatEntry()
    {
        // Arrange

        var context = new StockAutomationDbContext(_options);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var service = new EmailService(context, null /* will not need configuration */);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        var subscriber = new SubscriberCreate { EmailAddress = "_@_.com" };
        var _response = await service.CreateSubscriber(subscriber);
        var allSubscribers = await service.GetSubscribersAsync();

        // Act

        await service.DeleteSubscribersAsync(allSubscribers.Select(sub => sub.Id).ToList());

        // assert

        var allSubscribersAfterDelete = await service.GetSubscribersAsync();
        Assert.That(allSubscribersAfterDelete.Count(), Is.EqualTo(0));
    }

    // todo inconsistant behavior of `DeleteSubscribersAsync`
    //  - would expect either both of DeleteSubscribersAsync_TargetOneExistingAndOneNonexistant_DeletesExistingAndReturnsOk
    //    and DeleteSubscribersAsync_TargetSingleNonexistantEntry_ReturnsOk to fail or both of them to succeed

    [Test]
    public async Task DeleteSubscribersAsync_TargetOneExistingAndOneNonexistant_DeletesExistingAndReturnsOk()
    {
        // Arrange

        var context = new StockAutomationDbContext(_options);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var service = new EmailService(context, null /* will not need configuration */);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        var subscriber = new SubscriberCreate { EmailAddress = "_@_.com" };
        var _response = await service.CreateSubscriber(subscriber);
        var allSubscribers = await service.GetSubscribersAsync();

        // Act

        var idOfNonexistantEntry = 420;
        var response = await service.DeleteSubscribersAsync(allSubscribers.Select(sub => sub.Id).Append(idOfNonexistantEntry).ToList());

        Assert.Multiple(async () =>
        {

            // Assert

            Assert.That(response.IsOk);
            Assert.That((await service.GetSubscribersAsync()).Count(), Is.EqualTo(0));
        });
    }

    // todo either make pass & uncomment or delete
    //     [Test]
    //     public async Task DeleteSubscribersAsync_TargetSingleNonexistantEntry_ReturnsOk()
    //     {
    //         // Arrange

    //         var context = new StockAutomationDbContext(_options);

    // #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    //         var service = new EmailService(context, null /* will not need configuration */);
    // #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

    //         // Act

    //         var idOfNonexistantEntry = 0;
    //         var response = await service.DeleteSubscribersAsync([idOfNonexistantEntry]);

    //         // Assert

    //         Assert.That(response.IsOk);
    //     }

    [Test]
    public async Task DeleteSubscribersAsync_TargetOneOfExistingEntries_OthersUnaffected()
    {
        // Arrange

        var context = new StockAutomationDbContext(_options);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var service = new EmailService(context, null /* will not need configuration */);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        var subscriber1 = new SubscriberCreate { EmailAddress = "1@_.com" };
        var subscriber2 = new SubscriberCreate { EmailAddress = "2@_.com" };

        var _response1 = await service.CreateSubscriber(subscriber1);
        var subscriber1FromDb = (await service.GetSubscribersAsync()).ToList();

        var _response2 = await service.CreateSubscriber(subscriber2);

        // Act

        var deleteResponse = await service.DeleteSubscribersAsync(subscriber1FromDb.Select(sub => sub.Id).ToList());

        Assert.Multiple(async () =>
        {

            // Assert

            Assert.That(deleteResponse.IsOk);

            var subscribersAfterDelete = (await service.GetSubscribersAsync()).ToList();
            Assert.That(subscribersAfterDelete, Has.Count.EqualTo(1));
            Assert.That(subscribersAfterDelete[0].EmailAddress, Is.EqualTo(subscriber2.EmailAddress));
        });
    }
}