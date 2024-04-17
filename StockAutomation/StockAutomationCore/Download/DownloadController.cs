using StockAutomationCore.Configuration;
using StockAutomationCore.Files;

namespace StockAutomationCore.Download;

public class DownloadController
{
    public string DownloadUrl { get; set; }

    private readonly HttpClient _client = new();

    public DownloadController()
    {
        _client.DefaultRequestHeaders.Add("User-Agent", "StockAutomationCore/1.0");
        var defaultUrl = StockAutomationConfig.GetSection("download")["defaultUrl"];
        DownloadUrl = defaultUrl ?? throw new ArgumentNullException("No default URL specified in config");
    }

    public DownloadController(string url)
    {
        DownloadUrl = url;
    }

    public async Task<string> DownloadToFile()
    {
        var timestamp = DateTime.Now.ToString("s");
        var filename = $"snapshot-{timestamp}.csv";
        await using var streamResult = _client.GetStreamAsync(DownloadUrl).Result;
        await using var fs = new FileStream(Path.Join(FileUtils.SnapshotDir, filename), FileMode.CreateNew);
        await streamResult.CopyToAsync(fs);
        return filename;
    }

    public async Task<byte[]> DownloadToBytes()
    {
        using var stream = _client.GetStreamAsync(DownloadUrl);
        using var fileBytes = new MemoryStream((int)stream.Result.Length);
        await stream.Result.CopyToAsync(fileBytes);
        return fileBytes.ToArray();
    }
}
