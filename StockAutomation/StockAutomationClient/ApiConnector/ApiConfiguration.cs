using StockAutomationCore.Configuration;

namespace StockAutomationClient.ApiConnector;

public static class ApiConfiguration
{
    public static readonly string ApiUri = StockAutomationConfig.GetSection("Api")["defaultUrl"] ??
                                           "https://stock-automation.dyn.cloud.e-infra.cz/api";
}
