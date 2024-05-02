namespace StockAutomationCore.Download;

public class Downloader
{
    public static async Task<string> DownloadToFile(HttpClient client, string snapshotDir)
    {
        var timestamp = DateTime.Now.ToString("s");
        var filename = $"snapshot-{timestamp}.csv";
        await using var streamResult = client.GetStreamAsync(string.Empty).Result;
        await using var fs = new FileStream(Path.Join(snapshotDir, filename), FileMode.CreateNew);
        await streamResult.CopyToAsync(fs);
        await fs.FlushAsync();
        return filename;
    }

    public static async Task<byte[]> DownloadToBytes(HttpClient client)
    {
        var downloadUrl = client.BaseAddress!.ToString();

        string? maybeValidFilePath = null;
        if (downloadUrl.StartsWith("file://"))
        {
            maybeValidFilePath = downloadUrl[7..];
        }

        await using var streamResult = (maybeValidFilePath != null && File.Exists(maybeValidFilePath))
            ? File.OpenRead(maybeValidFilePath)
            : client.GetStreamAsync(downloadUrl).Result;

        await using var fileBytes = new MemoryStream(10240);  // should cover average snapshot size
        await streamResult.CopyToAsync(fileBytes);
        await fileBytes.FlushAsync();
        return fileBytes.ToArray();
    }
}
