using System.Net.Http.Json;
using BusinessLayer.Models;
using DataAccessLayer.Entities;

namespace StockAutomationClient.ApiConnector;

public static class SnapshotApi
{
    private static readonly HttpClient Client = new();
    private const string Endpoint = "Snapshot";

    public static async Task<List<HoldingSnapshot>> GetSnapshots()
    {
        var snapshots = new List<HoldingSnapshot>();
        var response = await Client.GetAsync($"{ApiConfiguration.ApiUri}/{Endpoint}");
        if (response.IsSuccessStatusCode)
        {
            snapshots = await response.Content.ReadFromJsonAsync<List<HoldingSnapshot>>() ?? [];
        }

        return snapshots;
    }

    public static async Task<string> DownloadSnapshot()
    {
        var response = await Client.GetAsync($"{ApiConfiguration.ApiUri}/{Endpoint}/Download");
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> CompareSnapshots(SnapshotCompare snapshotCompare)
    {
        var response = await Client.PostAsJsonAsync(
            $"{ApiConfiguration.ApiUri}/{Endpoint}/Compare", snapshotCompare);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> DeleteSnapshots(List<int> ids)
    {
        var response = await Client.PostAsJsonAsync(
            $"{ApiConfiguration.ApiUri}/{Endpoint}/Delete", ids);
        return await response.Content.ReadAsStringAsync();
    }
}