namespace StockAutomationCore.Error;

public class Usage
{
    public int PrintString()
    {
        var service = new Service();
        return service.GetString().Match(
            s =>
            {
                Console.WriteLine(s);
                return 0;
            },
            _ => 1);
    }
}