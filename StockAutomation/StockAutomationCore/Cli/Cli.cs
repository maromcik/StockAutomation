using Microsoft.Extensions.Configuration;
using Sharprompt;
using StockAutomationCore.EmailService;
using StockAutomationCore.Files;

namespace StockAutomationCore.Cli;

public class Cli
{
    private readonly EmailController _emailController = new();

    private readonly IConfiguration _configuration;

    public Cli(IConfiguration configuration)
    {
        _configuration = configuration;
        Prompt.ThrowExceptionOnCancel = true;
    }
    

    public void CliLoop()
    {
        Prompt.ThrowExceptionOnCancel = true;
        while (true)
        {
            try
            {
                CliSelector();
            }
            catch (PromptCanceledException)
            {
                return;
            }
            catch (SystemException)
            {
                Console.WriteLine($"An exception occured, make sure your terminal windows is large enough!");
                return;
            }
        }
    }

    private void CliSelector()
    {
        var value = Prompt.Select<Operation>("Select command");
        try
        {
            switch (value)
            {
                case Operation.File:
                    FileOperations();
                    break;
                case Operation.SnapshotDir:
                    SnapshotDirOperations();
                    break;
                case Operation.Download:
                    Console.WriteLine("Call downloader here");
                    // TODO: Call download
                    break;
                case Operation.Compare:
                    Console.WriteLine("Call diff here");
                    // TODO: Call diff
                    break;
                case Operation.Send:
                    SendEmail();
                    break;
                case Operation.Subscriber:
                    SubscriberOperations();
                    break;
                case Operation.Exit:
                    return;
                default:
                    Console.WriteLine("Unknown command");
                    break;
            }
        }
        catch (PromptCanceledException)
        {
        }
    }

    private static void SnapshotDirOperations()
    {
        while (true)
        {
            try
            {
                var value = Prompt.Select<SnapshotDirOperation>("Select directory command or return with CTRL + c");
                switch (value)
                {
                    case SnapshotDirOperation.Print:
                        Console.WriteLine(FileUtils.SnapshotDir);
                        break;
                    case SnapshotDirOperation.Change:
                        Console.WriteLine(ChangePath()
                            ? "Snapshot directory successfully changed"
                            : "Snapshot directory does not exist");
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
            }
            catch (PromptCanceledException)
            {
                return;
            }
        }
    }

    private static void FileOperations()
    {
        while (true)
        {
            try
            {
                var value = Prompt.Select<FileOperation>("Select file command or return with CTRL + c");   
                switch (value)
                {
                    case FileOperation.Print:
                        PrintFileList();
                        break;
                    case FileOperation.Delete:
                        DeleteFiles();
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
            }
            catch (PromptCanceledException)
            {
                return;
            }
        }
    }

    private void SubscriberOperations()
    {
        while (true)
        {
            try
            {
                var value = Prompt.Select<SubscriberOperation>("Select subscriber command or return with CTRL + c");
                switch (value)
                {
                    case SubscriberOperation.Print:
                        PrintSubscriberList();
                        break;
                    case SubscriberOperation.Add:
                        AddSubscriber();
                        break;
                    case SubscriberOperation.Delete:
                        DeleteSubscriber();
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
            }
            catch (PromptCanceledException)
            {
                return;
            }
        }
    }

    private static bool ChangePath()
    {
        var path = Prompt.Input<string>("Enter new snapshot directory");
        return FileUtils.ChangeSnapshotDir(path);
    }


    private static void PrintFileList()
    {
        FileUtils.GetFileList().MatchVoid(
            files =>
            {
                for (var i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"{i + 1}   {files[i].Name}   {files[i].LastWriteTime}");
                }
            },
            e => Console.WriteLine($"Error occured: {e}")
        );
    }

    private static void DeleteFiles()
    {
        var filesRes = FileUtils.GetFileList();
        if (!filesRes.IsOk)
        {
            Console.WriteLine($"Error occured: {filesRes.Error}");
            return;
        }

        var files = filesRes.Value;
        var toBeDeleted =
            Prompt.MultiSelect("Select files to be deleted", files, pageSize: 10, textSelector: f => f.Name);
        var isOk = Prompt.Confirm("Is this OK?");
        if (!isOk)
        {
            Console.WriteLine("No action performed");
            return;
        }

        FileUtils.DeleteFiles(toBeDeleted).MatchVoid(
            _ => Console.WriteLine("Selected files were deleted"),
            e => Console.WriteLine($"Error occured: {e}")
        );
    }

    private void AddSubscriber()
    {
        var email = Prompt.Input<string>("Enter email address of the new subscriber");
        _emailController.AddSubscriber(email);
        Console.WriteLine("Subscriber added");
    }

    private void PrintSubscriberList()
    {
        for (var i = 0; i < _emailController.Subscriptions.Count; i++)
        {
            Console.WriteLine($"{i + 1}   {_emailController.Subscriptions[i].EmailAddress}");
        }
    }

    private void DeleteSubscriber()
    {
        var toBeDeleted =
            Prompt.MultiSelect("Select files to be deleted", _emailController.Subscriptions, pageSize: 10, textSelector: s => s.EmailAddress);
        var isOk = Prompt.Confirm("Is this OK?");
        if (!isOk)
        {
            Console.WriteLine("No action performed");
            return;
        }
        _emailController.RemoveSubscribers(toBeDeleted);
        Console.WriteLine("Selected subscribers were deleted successfully");
    }

    private void SendEmail()
    {
        var diff = "42";
        try
        {
            _emailController.SendEmail(_configuration, diff);
            Console.WriteLine("Emails were successfully sent");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email. Error: {ex.Message}");
        }
        
    }
}