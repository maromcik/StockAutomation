// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using StockAutomationCore.EmailService;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

IConfigurationRoot configuration = builder.Build();


//email sending
var emailController = new EmailController();
emailController.AddSubscriber("ninarybarova29@gmail.com");

var diff = 42;
var email = emailController.SendEmail(configuration, $"<p> {diff} </p>");
if (email)
{
    Console.WriteLine("Emails were successfully sent.");
}
else
{
    Console.WriteLine("An error occured during sending emails.");
}