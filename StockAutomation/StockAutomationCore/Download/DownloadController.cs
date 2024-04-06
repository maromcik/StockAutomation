using StockAutomationCore.Files;

namespace StockAutomationCore.Download;

public class DownloadController(string downloadUrl)
{
    public string DownloadUrl { get; set; } = downloadUrl;

    private readonly HttpClient _client = new();

    public void DownloadToFile(string? filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            var timestamp = DateTime.Now.ToString("s");
            filename = $"snapshot-{timestamp}.csv";
        }

        using var stream = _client.GetStreamAsync(DownloadUrl);
        using var fs = new FileStream(Path.Join(FileUtils.Dir, filename), FileMode.CreateNew);
        stream.Result.CopyTo(fs);
    }

    public byte[] DownloadToBytes()
    {
        using var stream = _client.GetStreamAsync(DownloadUrl);
        using var fileBytes = new MemoryStream((int) stream.Result.Length);
        stream.Result.CopyTo(fileBytes);

        return fileBytes.ToArray();
    }
}
