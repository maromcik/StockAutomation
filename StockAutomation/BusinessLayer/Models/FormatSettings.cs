using DataAccessLayer.Entities;

namespace BusinessLayer.Models;

public class FormatSettings(OutputFormat format)
{
    public OutputFormat PreferredFormat { get; set; } = format;

    public FormatSettings() : this(OutputFormat.Text) {}
}
