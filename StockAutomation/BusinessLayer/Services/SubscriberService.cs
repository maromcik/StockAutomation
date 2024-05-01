using BusinessLayer.Errors;
using BusinessLayer.Models;
using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services;

using System.Net;
using System.Net.Mail;

public class SubscriberService : ISubscriberService
{
    private readonly StockAutomationDbContext _context;

    public SubscriberService(StockAutomationDbContext context)
    {
        _context = context;
        var config = _context.Configurations.FirstOrDefault();
        if (config == null)
        {
            _context.Configurations.Add(new Configuration
            {
                Id = 0,
                OutputFormat = OutputFormat.Text
            });
            _context.SaveChanges();
        }
    }

    public async Task<IEnumerable<Subscriber>> GetSubscribersAsync()
    {
        return await _context.Subscribers.ToListAsync();
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

        if (!IsEmailAddressValid(subscriberCreate.EmailAddress))
        {
            return new Error
            {
                ErrorType = ErrorType.InvalidEmailAddress,
                Message = "Invalid email address"
            };
        }

        var subscriber = new Subscriber
        {
            EmailAddress = subscriberCreate.EmailAddress
        };
        _context.Add(subscriber);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Result<bool, Error>> DeleteSubscribersAsync(List<int> ids)
    {
        var subscribers = await _context.Subscribers.Where(s => ids.Contains(s.Id)).ToListAsync();
        if (subscribers.Count == 0)
        {
            return new Error
            {
                ErrorType = ErrorType.NoSubscribersFound,
                Message = "Could not delete selected subscribers - not found"
            };
        }

        _context.Subscribers.RemoveRange(subscribers);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<SubscriberView> SearchSubscribersAsync(PaginationSettings? paginationSettings, string? query)
    {
        var subscribers = _context.Subscribers.AsQueryable();

        if (paginationSettings != null)
        {
            var subscribersCount = await subscribers.CountAsync();
            var pageCount = subscribersCount / paginationSettings.PageSize +
                            int.Min(subscribersCount % paginationSettings.PageSize, 1);
            subscribers = subscribers
                .Skip((paginationSettings.PageNumber - 1) * paginationSettings.PageSize)
                .Take(paginationSettings.PageSize);
            var result = await subscribers.ToListAsync();
            return new SubscriberView(result,
                paginationSettings.PageNumber, pageCount);
        }

        var result2 = await subscribers.ToListAsync();
        return new SubscriberView(result2,
            1, 1);
    }


    private static bool IsEmailAddressValid(string emailAddress)
    {
        try
        {
            var m = new MailAddress(emailAddress);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
