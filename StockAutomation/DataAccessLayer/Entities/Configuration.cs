using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities;

public class Configuration
{
    [Key]
    public required int Id { get; set; } = 1;
    public required string DownloadUrl { get; set; } = "";
    public required OutputFormat OutputFormat { get; set; } = OutputFormat.HTML;
}
