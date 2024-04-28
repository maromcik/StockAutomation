using DataAccessLayer.Entities;

namespace BusinessLayer.Models;

public class SubscriberView(IEnumerable<Subscriber> subscribers, int currentPage, int totalPages)
{
    public IEnumerable<Subscriber> Subscribers { get; init; } = subscribers;
    public int CurrentPage { get; set; } = currentPage;
    public int TotalPages { get; set; } = totalPages;
}
