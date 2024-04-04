// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using StockAutomationCore.EmailService;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

IConfigurationRoot configuration = builder.Build();


//email sending
var ninaSub = new Subscription { EmailAddress = "ninarybarova29@gmail.com"};

var diff = 42;
var email = EmailController.SendEmail(configuration, [ninaSub.EmailAddress], $"<p> {diff} </p>");
if (email)
{
    Console.WriteLine("Emails were successfully sent.");
}
else
{
    Console.WriteLine("An error occured during sending emails.");
}