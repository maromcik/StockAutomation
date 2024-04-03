namespace StockAutomationCore.Error;

public class Service
{
    public Result<string, Error> GetString()
    {
        var s = "null_string";
    
        if (s == null)
        {
            return Error.EmptyString;
        }

        return s;
    }

}