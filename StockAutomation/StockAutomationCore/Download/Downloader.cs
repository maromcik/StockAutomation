namespace StockAutomationCore.Download;

public static class Downloader
{
    public static async Task<string> DownloadToFile(HttpClient client, string downloadUrl, string snapshotDir)
    {
        var timestamp = DateTime.Now.ToString("s");
        var filename = $"snapshot-{timestamp}.csv";
        await using var streamResult = client.GetStreamAsync(downloadUrl).Result;
        await using var fs = new FileStream(Path.Join(snapshotDir, filename), FileMode.CreateNew);
        await streamResult.CopyToAsync(fs);
        await fs.FlushAsync();
        return filename;
    }

    public static async Task<byte[]> DownloadToBytes(HttpClient client, string downloadUrl)
    {
        await using var streamResult = client.GetStreamAsync(downloadUrl).Result;
        await using var fileBytes = new MemoryStream(10240);  // should cover average snapshot size
        await streamResult.CopyToAsync(fileBytes);
        await fileBytes.FlushAsync();
        return fileBytes.ToArray();
    }
}
