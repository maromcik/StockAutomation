using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models;

public class SubscriberCreate
{
    [Required]
    [EmailAddress]
    public required string EmailAddress { get; set; }
}
