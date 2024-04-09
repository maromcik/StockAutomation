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
                Console.WriteLine("An exception occured, make sure your terminal windows is large enough!");
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
                    Environment.Exit(0);
                    break;
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

    private static FileInfo[] FetchFiles()
    {
        try
        {
            return FileUtils.GetFileList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"File error occured: {e.Message}");
            return Array.Empty<FileInfo>();
        }
    }

    private static void PrintFileList()
    {
        var files = FetchFiles();
        for (var i = 0; i < files.Length; i++)
        {
            Console.WriteLine($"{i + 1}   {files[i].Name}   {files[i].LastWriteTime}");
        }
    }

    private static void DeleteFiles()
    {
        var files = FetchFiles();

        if (files.Length == 0)
        {
            Console.WriteLine("No files found");
            return;
        }

        var toBeDeleted =
            Prompt.MultiSelect("Select files to be deleted", files, pageSize: 10, textSelector: f => $"{f.Name}     {f.LastWriteTime}");
        var isOk = Prompt.Confirm("Is this OK?");
        if (!isOk)
        {
            Console.WriteLine("No action performed");
            return;
        }

        try
        {
            FileUtils.DeleteFiles(toBeDeleted);
            Console.WriteLine("Selected files were deleted");
        }

        catch (Exception e)
        {
            Console.WriteLine($"Error occured: {e.Message}");
        }
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
        var files = FetchFiles();

        if (files.Length < 2)
        {
            Console.WriteLine("You must download two or more files");
            return;
        }

        var oldFile = Prompt.Select<FileInfo>("Select old file", files, textSelector: f => $"{f.Name}     {f.LastWriteTime}");
        var newFile = Prompt.Select<FileInfo>("Select new file", files, textSelector: f => $"{f.Name}     {f.LastWriteTime}");

        DiffResult = DiffCalculator.GetDiffText(oldFile.ToString(), newFile.ToString());

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
        if (_emailController.Subscriptions.Count == 0)
        {
            Console.WriteLine("Subscriber list is empty");
            return;
        }
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
        catch (Exception e)
        {
            Console.WriteLine($"Failed to send email. Error: {e.Message}");
        }
    }
}
