namespace StockAutomationCore.Download;

public class DownloadController(string downloadUrl)
{
    public string DownloadUrl { get; set; } = downloadUrl;

    private readonly HttpClient client = new();

    public void DownloadToFile(string directoryName = "", string filename = "")
    {
        if (filename == "")
        {
            string timestamp = DateTime.Now.ToString("s");
            filename = $"snapshot-{timestamp}.csv";
        }

        using var stream = client.GetStreamAsync(DownloadUrl);
        using var fs = new FileStream(Path.Join(directoryName, filename), FileMode.CreateNew);
        stream.Result.CopyTo(fs);
    }

    public byte[] DownloadToBytes()
    {
        using var stream = client.GetStreamAsync(DownloadUrl);
        using var fileBytes = new MemoryStream((int) stream.Result.Length);
        stream.Result.CopyTo(fileBytes);

        return fileBytes.ToArray();
    }
}
