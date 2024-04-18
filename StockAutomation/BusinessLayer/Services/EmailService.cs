using BusinessLayer.Errors;
using BusinessLayer.Models;
using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services;

public class EmailService(StockAutomationDbContext context) : IEmailService
{
    public async Task<IEnumerable<Subscriber>> GetSubscribersAsync()
    {
        return await context.Subscribers.ToListAsync();
    }

    public async Task<Result<bool, Error>> CreateSubscriber(SubscriberCreate subscriberCreate)
    {
        if (string.IsNullOrEmpty(subscriberCreate.Email))
        {
            return new Error
            {
                ErrorType = ErrorType.EmailEmpty,
                Message = "Email address cannot be empty"
            };
        }

        // Validate emails
        var subscriber = new Subscriber
        {
            Email = subscriberCreate.Email
        };
        context.Add(subscriber);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Result<bool, Error>> DeleteSSubscribersAsync(List<int> ids)
    {
        var subscribers = await context.Subscribers.Where(s => ids.Contains(s.Id)).ToListAsync();
        if (subscribers.Count == 0)
        {
            return new Error
            {
                ErrorType = ErrorType.NoSubscribersFound,
                Message = "Could not delete selected subscribers - not found"
            };
        }

        context.Subscribers.RemoveRange(subscribers);
        await context.SaveChangesAsync();
        return true;
    }
}
