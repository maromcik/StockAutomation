using Microsoft.Extensions.Configuration;
using Sharprompt;
using StockAutomationCore.EmailService;

namespace StockAutomationCore.Cli;

public class Cli(IConfiguration configuration)
{
    private static string Dir { get; set; } = Directory.GetCurrentDirectory();
    private readonly EmailController _emailController = new EmailController();

    public void CliLoop()
    {
        while (true)
        {
            var value = Prompt.Select<Operation>("Select command");

            switch (value)
            {
                case Operation.PrintFiles:
                    PrintFileList();
                    break;
                case Operation.DeleteFiles:
                    DeleteFiles();
                    break;
                case Operation.Download:
                    break;
                case Operation.Compare:
                    Console.WriteLine("Call diff here");
                    break;
                case Operation.Send:
                    SendEmail();
                    break;
                case Operation.AddSubscriber:
                    AddSubscriber();
                    break;
                case Operation.PrintDir:
                    Console.WriteLine(Dir);
                    break;
                case Operation.ChangeDir:
                    Console.WriteLine(ChangePath() ? "Directory successfully changed" : "Invalid Path - Does not exist");
                    break;
                case Operation.Exit:
                    return;
                default:
                    Console.WriteLine("Unknown command");
                    break;
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
        var files = FileUtils.GetFileList(Dir);
        for (var i = 0; i < files.Length; i++)
        {
            Console.WriteLine($"{i + 1}   {files[i].Name}");   
        }
    }

    private static bool DeleteFiles()
    {

        var files = FileUtils.GetFileList(Dir);
        var toBeDeleted = Prompt.MultiSelect("Select files to be deleted", files, pageSize: 10, textSelector: f => f.Name);
        var isOk = Prompt.Confirm("Is this OK?");
        if (!isOk)
        {
            Console.WriteLine("No action performed");
            return false;
        }

        foreach (var file in toBeDeleted)
        {
            file.Delete();               
        }

        Console.WriteLine("Selected files were deleted");
        return true;
    }

    private void AddSubscriber()
    {
        var email = Prompt.Input<string>("Enter email address of the new subscriber");
        _emailController.AddSubscriber(email);
    }

    private void SendEmail()
    {
        var diff = 42;
        var email = _emailController.SendEmail(configuration, $"<p> {diff} </p>");
        Console.WriteLine(email ? "Emails were successfully sent." : "An error occured during sending emails.");
    }
}