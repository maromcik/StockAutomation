using System.Net.Http.Json;
using BusinessLayer.Models;

namespace StockAutomationClient.ApiConnector;

public static class EmailApi
{
    private static readonly HttpClient Client = new();
    private const string Endpoint = "Email";

    public static async Task<string> SendEmail(SnapshotCompare snapshotCompare)
    {
        var response = await Client.PostAsJsonAsync($"{ApiConfiguration.ApiUri}/{Endpoint}/Send", snapshotCompare);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> SendEmailLatest()
    {
        var response = await Client.GetAsync($"{ApiConfiguration.ApiUri}/{Endpoint}/SendLatest");
        return await response.Content.ReadAsStringAsync();
    }


    public static async Task<string> SaveEmailFormat(FormatSettings formatSettings)
    {
        var response = await Client.PostAsJsonAsync(
            $"{ApiConfiguration.ApiUri}/{Endpoint}/SaveSettings", formatSettings);
        return await response.Content.ReadAsStringAsync();
    }
}
