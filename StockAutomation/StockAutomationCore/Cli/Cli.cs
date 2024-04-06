using Microsoft.Extensions.Configuration;
using Sharprompt;
using StockAutomationCore.EmailService;

namespace StockAutomationCore.Cli;

public class Cli(IConfiguration configuration)
{
    private static string Dir { get; set; } = Directory.GetCurrentDirectory();
    private readonly EmailController _emailController = new();

    public void CliLoop()
    {
        Prompt.ThrowExceptionOnCancel = true;
        while (true)
        {
            var value = Prompt.Select<Operation>("Select command");
            try
            {
                switch (value)
                {
                    case Operation.File:
                        FileOperations();
                        break;
                    case Operation.WorkingDir:
                        WorkingDirOperations();
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
    }


    private static void WorkingDirOperations()
    {
        while (true)
        {
            var value = Prompt.Select<WorkingDirOperation>("Select directory command or return with CTRL + c");
            try
            {
                switch (value)
                {
                    case WorkingDirOperation.Print:
                        Console.WriteLine(Dir);
                        break;
                    case WorkingDirOperation.Change:
                        Console.WriteLine(ChangePath()
                            ? "Directory successfully changed"
                            : "Directory does not exist");
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
        var value = Prompt.Select<FileOperation>("Select file command or return with CTRL + c");
        while (true)
        {
            try
            {
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
        var path = Prompt.Input<string>("Enter new working directory");
        if (!Directory.Exists(path)) return false;
        Dir = path;
        return true;
    }


    private static void PrintFileList()
    {
        FileUtils.GetFileList(Dir).MatchVoid(
            files =>
            {
                for (var i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"{i + 1}   {files[i].Name}");
                }
            },
            e => Console.WriteLine($"Error occured: {e}")
        );
    }

    private static void DeleteFiles()
    {
        var filesRes = FileUtils.GetFileList(Dir);
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
        var diff = 42;
        var email = _emailController.SendEmail(configuration, $"<p> {diff} </p>");
        Console.WriteLine(email ? "Emails were successfully sent." : "An error occured during sending emails.");
    }
}