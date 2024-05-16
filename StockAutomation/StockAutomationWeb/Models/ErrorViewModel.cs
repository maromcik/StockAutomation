using BusinessLayer.Errors;

namespace StockAutomationWeb.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public ErrorType ErrorType { get; set; }
    public string? Message { get; set; }
}