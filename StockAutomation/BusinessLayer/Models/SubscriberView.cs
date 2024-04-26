using DataAccessLayer.Entities;

namespace BusinessLayer.Models;

public class SubscriberView
{
    public IEnumerable<Subscriber> Subscribers { get; init; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    
    
    public SubscriberView(IEnumerable<Subscriber> subscribers, int currentPage, int totalPages)
    {
        Subscribers = subscribers;
        CurrentPage = currentPage;
        TotalPages = totalPages;
    }
    
}