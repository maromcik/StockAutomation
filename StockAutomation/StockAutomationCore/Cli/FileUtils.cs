using StockAutomationCore.Error;

namespace StockAutomationCore.Cli;

public static class FileUtils
{
    public static FileInfo[] GetFileList(string path)
    {
        var d = new DirectoryInfo(path);
        var files =  d.GetFiles("*.json");
        return files.OrderByDescending(f => f.LastWriteTime).ToArray();
    }
}