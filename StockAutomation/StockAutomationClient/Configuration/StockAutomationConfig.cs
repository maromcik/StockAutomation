using Microsoft.Extensions.Configuration;

namespace StockAutomationClient.Configuration;

public static class StockAutomationConfig
{
    private static readonly IConfiguration Configuration;

    static StockAutomationConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        Configuration = builder.Build();
    }

    public static IConfigurationSection GetSection(string section)
    {
        return Configuration.GetSection(section);
    }
}
