using System.Net.Http.Json;
using BusinessLayer.Models;
using DataAccessLayer.Entities;

namespace StockAutomationClient.ApiConnector;

public static class SubscriberApi
{
    private static readonly HttpClient Client = new();
    private const string Endpoint = "Subscriber";

    public static async Task<List<Subscriber>> GetSubscribers()
    {
        var emails = new List<Subscriber>();
        var response = await Client.GetAsync($"{ApiConfiguration.ApiUri}/{Endpoint}");
        if (response.IsSuccessStatusCode)
        {
            emails = await response.Content.ReadFromJsonAsync<List<Subscriber>>() ?? [];
        }

        return emails;
    }


    public static async Task<string> CreateSubscriber(SubscriberCreate subscriberCreate)
    {
        var response = await Client.PostAsJsonAsync(
            $"{ApiConfiguration.ApiUri}/{Endpoint}/CreateSubscriber", subscriberCreate);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> DeleteSubscribers(List<int> ids)
    {
        var response = await Client.PostAsJsonAsync(
            $"{ApiConfiguration.ApiUri}/{Endpoint}/Delete", ids);
        return await response.Content.ReadAsStringAsync();
    }
}
