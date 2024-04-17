using StockAutomationClient.Cli;
using StockAutomationCore.Files;

if (!FileUtils.CreateSnapshotDir(FileUtils.SnapshotDir))
{
    Console.WriteLine("Snapshot directory could not be created, check your file permissions");
    return;
}

var cli = new Cli();
await cli.CliLoop();
