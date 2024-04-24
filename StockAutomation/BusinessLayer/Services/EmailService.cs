using BusinessLayer.Errors;
using BusinessLayer.Models;
using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessLayer.Services;

public class EmailService(StockAutomationDbContext context, IConfiguration configuration) : IEmailService
{
    public async Task<IEnumerable<Subscriber>> GetSubscribersAsync()
    {
        return await context.Subscribers.ToListAsync();
    }

    public async Task<Result<bool, Error>> SendEmailAsync()
    {
        var subscribers = await context.Subscribers.ToListAsync();
        Console.WriteLine(configuration.GetSection("SMTP")["Username"]);
        return true;
    }

    public async Task<Result<bool, Error>> CreateSubscriber(SubscriberCreate subscriberCreate)
    {
        if (string.IsNullOrEmpty(subscriberCreate.EmailAddress))
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
            EmailAddress = subscriberCreate.EmailAddress
        };
        context.Add(subscriber);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Result<bool, Error>> DeleteSubscribersAsync(List<int> ids)
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
