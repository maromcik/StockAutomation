using BusinessLayer.Errors;
using BusinessLayer.Models;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services;

public interface IEmailService
{
    public Task<IEnumerable<Subscriber>> GetSubscribersAsync();
    public Task<Result<bool, Error>> CreateSubscriber(SubscriberCreate subscriberCreate);
    public Task<Result<bool, Error>> DeleteSSubscribersAsync(List<int> ids);
}
