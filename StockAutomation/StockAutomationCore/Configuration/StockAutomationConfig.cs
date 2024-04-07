using Microsoft.Extensions.Configuration;

namespace StockAutomationCore.Configuration;

public static class StockAutomationConfig
{
    private static readonly IConfiguration _configuration;

    static StockAutomationConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        _configuration = builder.Build();
    }

    public static IConfigurationSection GetSection(string section)
    {
        return _configuration.GetSection(section);
    }
}
