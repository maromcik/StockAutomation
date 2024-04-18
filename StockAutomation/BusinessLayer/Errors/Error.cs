namespace BusinessLayer.Errors;

public class Error
{
    public ErrorType ErrorType { get; set; }
    public string Message { get; set; } = "Unknown error";
}
