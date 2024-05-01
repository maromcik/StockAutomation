using BusinessLayer.Models;
using DataAccessLayer.Entities;
using Sharprompt;
using StockAutomationClient.ApiConnector;

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
                case Operation.Snapshot:
                    await SnapshotOperations();
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


    private async Task SnapshotOperations()
    {
        while (true)
        {
            try
            {
                var value = Prompt.Select<SnapshotOperation>("Select snapshot command or return with CTRL + c");
                switch (value)
                {
                    case SnapshotOperation.Print:
                        await PrintSnapshotList();
                        break;
                    case SnapshotOperation.Delete:
                        await DeleteSnapshots();
                        break;
                    case SnapshotOperation.Download:
                        await DownloadSnapshot();
                        break;
                    case SnapshotOperation.Compare:
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
                        await SendEmail();
                        break;
                    case EmailOperation.SendLatest:
                        await SendEmailLatest();
                        break;
                    case EmailOperation.ChangeFormat:
                        await ChangeFormat();
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

    private static async Task<List<HoldingSnapshot>> FetchSnapshots()
    {
        return (await SnapshotApi.GetSnapshots()).ToList();
    }

    private static async Task PrintSnapshotList()
    {
        var snapshots = await FetchSnapshots();
        for (var i = 0; i < snapshots.Count; i++)
        {
            Console.WriteLine($"  {i + 1}    {snapshots[i].DownloadedAt}");
        }
    }

    private static async Task DeleteSnapshots()
    {
        var snapshots = await FetchSnapshots();

        if (snapshots.Count == 0)
        {
            Console.WriteLine("No snapshots found");
            return;
        }

        var toBeDeleted =
            Prompt.MultiSelect("Select snapshots to be deleted", snapshots, pageSize: 10,
                textSelector: f => $"{f.DownloadedAt}").Select(s => s.Id).ToList();
        var isOk = Prompt.Confirm("Is this OK?");
        if (!isOk)
        {
            Console.WriteLine("No action performed");
            return;
        }

        Console.WriteLine(await SnapshotApi.DeleteSnapshots(toBeDeleted));
    }

    private async Task DownloadSnapshot()
    {
        Console.WriteLine(await SnapshotApi.DownloadSnapshot());
    }

    private async Task<(bool result, SnapshotCompare snapshotCompare)> ChooseSnapshots()
    {
        var snapshots = await FetchSnapshots();

        if (snapshots.Count < 2)
        {
            Console.WriteLine("You must download two or more snapshots");
            return (false, new SnapshotCompare());
        }

        var newSnapshot = Prompt.Select("Select new snapshot", snapshots,
            textSelector: f => $"{f.DownloadedAt}");

        snapshots.Remove(newSnapshot);

        var oldSnapshot = Prompt.Select("Select old snapshot", snapshots,
            textSelector: f => $"{f.DownloadedAt}");
        return (true, new SnapshotCompare
        {
            NewId = newSnapshot.Id,
            OldId = oldSnapshot.Id
        });
    }

    private async Task Compare()
    {
        var chooseSnapshots = await ChooseSnapshots();
        if (!chooseSnapshots.result)
        {
            return;
        }

        var diff = await SnapshotApi.CompareSnapshots(chooseSnapshots.snapshotCompare);
        Console.WriteLine($"Differences:\n{diff}");
    }

    private async Task AddSubscriber()
    {
        var email = Prompt.Input<string>("Enter email address of the new subscriber");
        var response = await EmailApi.CreateSubscriber(new SubscriberCreate
        {
            EmailAddress = email
        });
        Console.WriteLine(response);
    }

    private async Task PrintSubscriberList()
    {
        var subscribers = await EmailApi.GetSubscribers();
        for (var i = 0; i < subscribers.Count; i++)
        {
            Console.WriteLine($"{i + 1}   {subscribers[i].EmailAddress}");
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
            Prompt.MultiSelect("Select snapshots to be deleted", subscribers, pageSize: 10,
                textSelector: s => s.EmailAddress).Select(e => e.Id).ToList();
        var isOk = Prompt.Confirm("Is this OK?");
        if (!isOk)
        {
            Console.WriteLine("No action performed");
            return;
        }

        var response = await EmailApi.DeleteSubscribers(toBeDeleted);
        Console.WriteLine(response);
    }

    private async Task SendEmail()
    {
        var chooseSnapshots = await ChooseSnapshots();
        if (!chooseSnapshots.result)
        {
            return;
        }

        var response = await EmailApi.SendEmail(chooseSnapshots.snapshotCompare);
        Console.WriteLine(response);
    }

    private async Task SendEmailLatest()
    {
        var response = await EmailApi.SendEmailLatest();
        Console.WriteLine(response);
    }

    private async Task ChangeFormat()
    {
        var format = Prompt.Select<OutputFormat>("Select format");
        await EmailApi.SaveEmailFormat(new FormatSettings
        {
            PreferredFormat = format
        });
    }
}
