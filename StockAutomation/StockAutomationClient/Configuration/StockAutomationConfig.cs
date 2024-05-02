using Microsoft.Extensions.Configuration;

namespace StockAutomationClient.Configuration;

public static class StockAutomationConfig
{
    private static readonly IConfiguration Configuration;

    static StockAutomationConfig()
    {
        var path = $"{Directory.GetCurrentDirectory()}/../../../";
        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        Configuration = builder.Build();
    }

    public static IConfigurationSection GetSection(string section)
    {
        return Configuration.GetSection(section);
    }
}
