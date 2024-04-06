using StockAutomationCore.Error;

namespace StockAutomationCore.Cli;

public static class FileUtils
{
    public static Result<FileInfo[], ErrorType> GetFileList(string path)
    {
        var d = new DirectoryInfo(path);
        try
        {
            var files = d.GetFiles("*.json");
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