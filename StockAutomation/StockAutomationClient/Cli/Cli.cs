using BusinessLayer.Models;
using Sharprompt;
using StockAutomationClient.ApiConnector;
using StockAutomationClient.Models;

namespace StockAutomationClient.Cli;

public class Cli
{
    public Cli()
    {
        Prompt.ThrowExceptionOnCancel = true;
    }

    public async Task CliLoop()
    {
        Prompt.ThrowExceptionOnCancel = true;
        while (true)
        {
            try
            {
                await CliSelector();
            }
            catch (PromptCanceledException)
            {
                return;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(
                    $"Could not connect to the API server\nOriginal message: {e.Message}");
            }
            catch (SystemException e)
            {
                Console.WriteLine(
                    $"An exception occured, make sure your terminal windows is large enough!\nOriginal message: {e.Message}");
                return;
            }
        }
    }

    private async Task CliSelector()
    {
        var value = Prompt.Select<Operation>("Select command");
        try
        {
            switch (value)
            {
                case Operation.File:
                    await FileOperations();
                    break;
                case Operation.Email:
                    await EmailOperations();
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


    private async Task FileOperations()
    {
        while (true)
        {
            try
            {
                var value = Prompt.Select<FileOperation>("Select file command or return with CTRL + c");
                switch (value)
                {
                    case FileOperation.Print:
                        await PrintFileList();
                        break;
                    case FileOperation.Delete:
                        await DeleteFiles();
                        break;
                    case FileOperation.Download:
                        await DownloadFile();
                        break;
                    case FileOperation.Compare:
                        await Compare();
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

    private async Task EmailOperations()
    {
        while (true)
        {
            try
            {
                var value = Prompt.Select<EmailOperation>("Select email command or return with CTRL + c");
                switch (value)
                {
                    case EmailOperation.Print:
                        await PrintSubscriberList();
                        break;
                    case EmailOperation.Add:
                        await AddSubscriber();
                        break;
                    case EmailOperation.Delete:
                        await DeleteSubscriber();
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


    private static async Task<List<Snapshot>> FetchFiles()
    {
        return (await SnapshotApi.GetSnapshots()).ToList();
    }

    private static async Task PrintFileList()
    {
        var files = await FetchFiles();
        for (var i = 0; i < files.Count; i++)
        {
            Console.WriteLine($"{i + 1}   {files[i].FilePath}   {files[i].DownloadedAt}");
        }
    }

    private static async Task DeleteFiles()
    {
        var files = await FetchFiles();

        if (files.Count == 0)
        {
            Console.WriteLine("No files found");
            return;
        }

        var toBeDeleted =
            Prompt.MultiSelect("Select files to be deleted", files, pageSize: 10,
                textSelector: f => $"{f.FilePath}     {f.DownloadedAt}").Select(s => s.Id).ToList();
        var isOk = Prompt.Confirm("Is this OK?");
        if (!isOk)
        {
            Console.WriteLine("No action performed");
            return;
        }

        Console.WriteLine(await SnapshotApi.DeleteSnapshots(toBeDeleted));
    }

    private async Task DownloadFile()
    {
        Console.WriteLine(await SnapshotApi.DownloadSnapshot());
    }

    private async Task Compare()
    {
        var files = await FetchFiles();

        if (files.Count < 2)
        {
            Console.WriteLine("You must download two or more files");
            return;
        }

        var newFile = Prompt.Select<Snapshot>("Select new file", files,
            textSelector: f => $"{f.FilePath}     {f.DownloadedAt}");

        files.Remove(newFile);

        var oldFile = Prompt.Select<Snapshot>("Select old file", files,
            textSelector: f => $"{f.FilePath}     {f.DownloadedAt}");
        var diff = await SnapshotApi.CompareSnapshots(new SnapshotCompare
        {
            NewId = newFile.Id,
            OldId = oldFile.Id
        });
        Console.WriteLine($"Differences:\n{diff}");
    }

    private async Task AddSubscriber()
    {
        var email = Prompt.Input<string>("Enter email address of the new subscriber");
        var response = await EmailApi.CreateSubscriber(new SubscriberCreate
        {
            Email = email
        });
        Console.WriteLine(response);
    }

    private async Task PrintSubscriberList()
    {
        var subscribers = await EmailApi.GetSubscribers();
        for (var i = 0; i < subscribers.Count; i++)
        {
            Console.WriteLine($"{i + 1}   {subscribers[i].Email}");
        }
    }

    private async Task DeleteSubscriber()
    {
        var subscribers = await EmailApi.GetSubscribers();
        if (subscribers.Count == 0)
        {
            Console.WriteLine("Subscriber list is empty");
            return;
        }

        var toBeDeleted =
            Prompt.MultiSelect("Select files to be deleted", subscribers, pageSize: 10,
                textSelector: s => s.Email).Select(e => e.Id).ToList();
        var isOk = Prompt.Confirm("Is this OK?");
        if (!isOk)
        {
            Console.WriteLine("No action performed");
            return;
        }

        var response = await EmailApi.DeleteSubscribers(toBeDeleted);
        Console.WriteLine(response);
    }

    private void SendEmail()
    {
        try
        {
            Console.WriteLine("Emails were successfully sent");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to send email. Error: {e.Message}");
        }
    }
}
