namespace StockAutomationCore.Error;

public class Service
{
    public Result<string, ErrorType> GetString()
    {
        var s = "null_string";
    
        if (s == null)
        {
            return ErrorType.EmptyString;
        }

        return s;
    }

}