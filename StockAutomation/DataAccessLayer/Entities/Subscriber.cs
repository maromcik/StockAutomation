namespace DataAccessLayer.Entities;

public class Subscriber : BaseEntity
{
    public required string EmailAddress { get; set; }
}