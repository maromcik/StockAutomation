using Microsoft.Extensions.Configuration;

namespace StockAutomationCore.Configuration;

public static class StockAutomationConfig
{
    public static readonly IConfiguration Configuration;

    static StockAutomationConfig()
    {
        var projectDir = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ??
                         Directory.GetCurrentDirectory();

        var builder = new ConfigurationBuilder()
            .SetBasePath(projectDir)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        Configuration = builder.Build();
    }

    public static IConfigurationSection GetSection(string section)
    {
        return Configuration.GetSection(section);
    }
}