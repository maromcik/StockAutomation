namespace BusinessLayer.Models;

public class FormatSettings
{
    public string PreferredFormat { get; set; }
    
    public FormatSettings(string format)
    {
        PreferredFormat = format;
    }
    
    public FormatSettings() {}
}