using StockAutomationCore.Error;

namespace StockAutomationCore.Files;

public static class FileUtils
{
    public static string SearchPattern { get; set; } = "*.csv";
    public static string SnapshotDir { get; set; } = Directory.GetCurrentDirectory() + "/snapshots";

    public static bool CreateSnapshotDir()
    {
        try
        {
            Directory.CreateDirectory(SnapshotDir);
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
    
    public static Result<FileInfo[], ErrorType> GetFileList()
    {
        var d = new DirectoryInfo(SnapshotDir);
        try
        {
            var files = d.GetFiles(SearchPattern);
            return files.OrderByDescending(f => f.LastWriteTime).ToArray();
        }
        catch (DirectoryNotFoundException)
        {
            return ErrorType.DoesNotExist;
        }
        catch (System.Security.SecurityException)
        {
            return ErrorType.PermissionError;
        }
        catch (UnauthorizedAccessException)
        {
            return ErrorType.Unauthorized;
        }
        catch (IOException)
        {
            return ErrorType.IoError;
        }
        
    }

    public static Result<bool, ErrorType> DeleteFiles(IEnumerable<FileInfo> files)
    {
        foreach (var file in files)
        {
            try
            {
                file.Delete();
            }
            catch (System.Security.SecurityException)
            {
                return ErrorType.PermissionError;
            }
            catch (UnauthorizedAccessException)
            {
                return ErrorType.Unauthorized;
            }
            catch (IOException)
            {
                return ErrorType.IoError;
            }
        }

        return true;
    }
}