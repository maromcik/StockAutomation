using System.Net.Http.Json;
using BusinessLayer.Models;
using DataAccessLayer.Entities;

namespace StockAutomationClient.ApiConnector;

public static class EmailApi
{
    private static readonly HttpClient Client = new();

    public static async Task<List<Subscriber>> GetSubscribers()
    {
        var emails = new List<Subscriber>();
        var response = await Client.GetAsync($"{ApiConfiguration.ApiUri}/Email");
        if (response.IsSuccessStatusCode)
        {
            emails = await response.Content.ReadFromJsonAsync<List<Subscriber>>() ?? [];
        }

        return emails;
    }

    public static async Task<string> SendEmail(SnapshotCompare snapshotCompare)
    {
        var response = await Client.PostAsJsonAsync($"{ApiConfiguration.ApiUri}/Email/Send", snapshotCompare);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> SendEmailLatest()
    {
        var response = await Client.GetAsync($"{ApiConfiguration.ApiUri}/Email/SendLatest");
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> CreateSubscriber(SubscriberCreate subscriberCreate)
    {
        var response = await Client.PostAsJsonAsync(
            $"{ApiConfiguration.ApiUri}/Email", subscriberCreate);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> DeleteSubscribers(List<int> ids)
    {
        var response = await Client.PostAsJsonAsync(
            $"{ApiConfiguration.ApiUri}/Email/Delete", ids);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> GetSupportedFormats()
    {
        var response = await Client.GetAsync(
            $"{ApiConfiguration.ApiUri}/Email/SupportedFormats");
        return await response.Content.ReadAsStringAsync();
    }
}
