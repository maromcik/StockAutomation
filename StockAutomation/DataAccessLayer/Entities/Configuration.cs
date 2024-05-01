using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities;

public class Configuration
{
    [Key]
    public required int Id { get; init; } = 1;
    public required OutputFormat OutputFormat { get; set; } = OutputFormat.HTML;
}
