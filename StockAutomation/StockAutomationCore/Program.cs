using StockAutomationCore.Cli;
using StockAutomationCore.Files;

if (!FileUtils.CreateSnapshotDir())
{
    Console.WriteLine("Snapshot directory could not be created, check your file permissions");
    return;
}

var cli = new Cli();
cli.CliLoop();