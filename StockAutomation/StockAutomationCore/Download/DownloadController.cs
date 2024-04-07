using StockAutomationCore.Configuration;
using StockAutomationCore.Files;

namespace StockAutomationCore.Download;

public class DownloadController
{
    public string DownloadUrl { get; set; }

    private readonly HttpClient _client = new();

    public DownloadController()
    {
        var defaultUrl = StockAutomationConfig.GetSection("download")["defaultUrl"];
        DownloadUrl = defaultUrl ?? throw new ArgumentNullException("No default URL specified in config");
    }

    public void DownloadToFile(string? filename = null)
    {
        if (string.IsNullOrEmpty(filename))
        {
            var timestamp = DateTime.Now.ToString("s");
            filename = $"snapshot-{timestamp}.csv";
        }

        using var streamResult = _client.GetStreamAsync(DownloadUrl).Result;
        using var fs = new FileStream(Path.Join(FileUtils.SnapshotDir, filename), FileMode.CreateNew);
        streamResult.CopyTo(fs);
    }

    public byte[] DownloadToBytes()
    {
        using var stream = _client.GetStreamAsync(DownloadUrl);
        using var fileBytes = new MemoryStream((int)stream.Result.Length);
        stream.Result.CopyTo(fileBytes);

        return fileBytes.ToArray();
    }
}
