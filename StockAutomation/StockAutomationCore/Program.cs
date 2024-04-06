using Microsoft.Extensions.Configuration;
using StockAutomationCore.Cli;
using StockAutomationCore.Files;


var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var configuration = builder.Build();

if (!FileUtils.CreateSnapshotDir())
{
    Console.WriteLine("Snapshot directory could not be created, check your file permissions");
    return;
}

var cli = new Cli(configuration);
cli.CliLoop();