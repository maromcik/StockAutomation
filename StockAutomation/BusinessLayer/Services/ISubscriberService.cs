using BusinessLayer.Errors;
using BusinessLayer.Models;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services;

public interface ISubscriberService
{
    public Task<IEnumerable<Subscriber>> GetSubscribersAsync();

    public Task<SubscriberView> SearchSubscribersAsync(PaginationSettings? paginationSettings, string? query);

    public Task<Result<bool, Error>> CreateSubscriber(SubscriberCreate subscriberCreate);
    public Task<Result<bool, Error>> DeleteSubscribersAsync(List<int> ids);
}
