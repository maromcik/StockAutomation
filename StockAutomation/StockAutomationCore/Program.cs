using Microsoft.Extensions.Configuration;
using StockAutomationCore.Cli;


var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var configuration = builder.Build();

var cli = new Cli(configuration);
cli.CliLoop();