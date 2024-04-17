namespace StockAutomationCore.Files;

public static class FileUtils
{
    public static string SearchPattern { get; set; } = "*.csv";
    public static string SnapshotDir { get; set; } = Directory.GetCurrentDirectory() + "/snapshots";

    public static bool CreateSnapshotDir(string snapshotDir)
    {
        try
        {
            Directory.CreateDirectory(snapshotDir);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool ChangeSnapshotDir(string path)
    {
        if (!Directory.Exists(path))
        {
            return false;
        }

        SnapshotDir = path;
        return true;
    }

    public static FileInfo[] GetFileList()
    {
        var d = new DirectoryInfo(SnapshotDir);

        var files = d.GetFiles(SearchPattern);
        return files.OrderByDescending(f => f.LastWriteTime).ToArray();
    }

    public static void DeleteFiles(IEnumerable<FileInfo> files)
    {
        foreach (var file in files)
        {
            file.Delete();
        }
    }
}
