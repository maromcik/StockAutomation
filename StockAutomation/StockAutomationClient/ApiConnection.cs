using System.Net.Http.Json;
using BusinessLayer.Models;
using StockAutomationClient.Models;

namespace StockAutomationClient;

public static class ApiConnection
{
    private static string ApiUri = "http://localhost:5401";
    private static readonly HttpClient Client = new();

    public static async Task<List<Snapshot>> GetSnapshots()
    {
        var snapshots = new List<Snapshot>();
        var response = await Client.GetAsync($"{ApiUri}/Snapshot");
        if (response.IsSuccessStatusCode)
        {
            snapshots = await response.Content.ReadFromJsonAsync<List<Snapshot>>() ?? [];
        }

        return snapshots;
    }

    public static async Task<string> DownloadSnapshot()
    {
        var response = await Client.GetAsync($"{ApiUri}/Snapshot/Download");
        if (response.IsSuccessStatusCode)
        {
            return "Successfully downloaded";
        }

        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> CompareSnapshots(SnapshotCompare snapshotCompare)
    {
        var response = await Client.PostAsJsonAsync(
            $"{ApiUri}/Snapshot/Compare", snapshotCompare);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> DeleteSnapshots(List<int> ids)
    {
        var response = await Client.PostAsJsonAsync(
            $"{ApiUri}/Snapshot/Delete", ids);
        if (response.IsSuccessStatusCode)
        {
            return "Successfully deleted";
        }

        return await response.Content.ReadAsStringAsync();
    }
}
