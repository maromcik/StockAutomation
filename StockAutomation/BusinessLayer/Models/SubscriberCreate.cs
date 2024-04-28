using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models;

public class SubscriberCreate
{
    [Required]
    public required string EmailAddress { get; set; }
}
