using System.Net.Http.Json;
using BusinessLayer.Models;
using DataAccessLayer.Entities;

namespace StockAutomationClient.ApiConnector;

public static class SnapshotApi
{
    private static readonly HttpClient Client = new();

    public static async Task<List<HoldingSnapshot>> GetSnapshots()
    {
        var snapshots = new List<HoldingSnapshot>();
        var response = await Client.GetAsync($"{ApiConfiguration.ApiUri}/Snapshot");
        if (response.IsSuccessStatusCode)
        {
            snapshots = await response.Content.ReadFromJsonAsync<List<HoldingSnapshot>>() ?? [];
        }

        return snapshots;
    }

    public static async Task<string> DownloadSnapshot()
    {
        var response = await Client.GetAsync($"{ApiConfiguration.ApiUri}/Snapshot/Download");
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> CompareSnapshots(SnapshotCompare snapshotCompare)
    {
        var response = await Client.PostAsJsonAsync(
            $"{ApiConfiguration.ApiUri}/Snapshot/Compare", snapshotCompare);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> DeleteSnapshots(List<int> ids)
    {
        var response = await Client.PostAsJsonAsync(
            $"{ApiConfiguration.ApiUri}/Snapshot/Delete", ids);
        return await response.Content.ReadAsStringAsync();
    }
}