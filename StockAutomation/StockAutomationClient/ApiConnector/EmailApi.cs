using System.Net.Http.Json;
using StockAutomationClient.Models;
using StockAutomationCore.Configuration;
using StockAutomationConfig = StockAutomationClient.Configuration.StockAutomationConfig;

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
    
    public static async Task<string> SendEmail()
    {
        var response = await Client.GetAsync($"{ApiConfiguration.ApiUri}/Email/Send");
        if (response.IsSuccessStatusCode)
        {
            return "Emails were successfully sent";
        }
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> CreateSubscriber(SubscriberCreate subscriberCreate)
    {
        var response = await Client.PostAsJsonAsync(
            $"{ApiConfiguration.ApiUri}/Email", subscriberCreate);
        if (response.IsSuccessStatusCode)
        {
            return "Successfully created";
        }
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> DeleteSubscribers(List<int> ids)
    {
        var response = await Client.PostAsJsonAsync(
            $"{ApiConfiguration.ApiUri}/Email/Delete", ids);
        if (response.IsSuccessStatusCode)
        {
            return "Successfully deleted";
        }

        return await response.Content.ReadAsStringAsync();
    }
}
