using StockAutomationClient.Configuration;

namespace StockAutomationClient.ApiConnector;

public static class ApiConfiguration
{
    public static readonly string ApiUri = StockAutomationConfig.GetSection("Api")["defaultUrl"] ??
                                           "http://localhost:5401";
}
