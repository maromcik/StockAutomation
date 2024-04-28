namespace BusinessLayer.Errors;

public class Error
{
    public ErrorType ErrorType { get; init; }
    public string Message { get; init; } = "Unknown error";
}
