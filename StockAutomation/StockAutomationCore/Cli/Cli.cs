using Microsoft.Extensions.Configuration;
using Sharprompt;
using StockAutomationCore.Download;
using StockAutomationCore.Diff;
using StockAutomationCore.EmailService;
using StockAutomationCore.Files;

namespace StockAutomationCore.Cli;

public class Cli
{
    private readonly EmailController _emailController = new();
    private readonly DownloadController _downloadController = new();

    private string? DiffResult { get; set; }

    public Cli()
    {
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
                case Operation.Email:
                    EmailOperations();
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

    private void FileOperations()
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
                    case FileOperation.Download:
                        DownloadFile();
                        break;
                    case FileOperation.Compare:
                        Compare();
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

    private void EmailOperations()
    {
        while (true)
        {
            try
            {
                var value = Prompt.Select<EmailOperation>("Select email command or return with CTRL + c");
                switch (value)
                {
                    case EmailOperation.Print:
                        PrintSubscriberList();
                        break;
                    case EmailOperation.Add:
                        AddSubscriber();
                        break;
                    case EmailOperation.Delete:
                        DeleteSubscriber();
                        break;
                    case EmailOperation.Send:
                        SendEmail();
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
        var files = FileUtils.GetFileList();
        if (!files.IsOk)
        {
            Console.WriteLine($"Error occured: {files.Error}");
            return;
        }

        if (files.Value.Length == 0)
        {
            Console.WriteLine("No files found");
            return;
        }

        var toBeDeleted =
            Prompt.MultiSelect("Select files to be deleted", files.Value, pageSize: 10, textSelector: f => f.Name);
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

    private void DownloadFile()
    {
        try
        {
            _downloadController.DownloadToFile();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during file download: {e.Message}");
        }
    }
    
    private void Compare()
    {
        var files = FileUtils.GetFileList();
        if (!files.IsOk)
        {
            Console.WriteLine($"Error occured: {files.Error}");
            return;
        }

        if (files.Value.Length == 0)
        {
            Console.WriteLine("No files found");
            return;
        }

        var comparePair = Prompt
            .MultiSelect("Select files to be compared", files.Value, minimum: 2, maximum: 2, pageSize: 10,
                textSelector: f => f.Name).OrderByDescending(f => f.LastWriteTime)
            .ToList();

        if (comparePair.Count != 2)
        {
            Console.WriteLine("Invalid number of files selected. Please select exactly 2 files.");
            return;
        }

        DiffResult = DiffCalculator.GetDiffText(comparePair[0].ToString(), comparePair[1].ToString());

        Console.WriteLine($"Differences:\n{DiffResult}");
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
            Prompt.MultiSelect("Select files to be deleted", _emailController.Subscriptions, pageSize: 10,
                textSelector: s => s.EmailAddress);
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
        if (string.IsNullOrEmpty(DiffResult))
        {
            Console.WriteLine("Diff is empty, make sure to run Compare first");
            return;
        }

        try
        {
            _emailController.SendEmail(DiffResult);
            Console.WriteLine("Emails were successfully sent");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email. Error: {ex.Message}");
        }
    }
}