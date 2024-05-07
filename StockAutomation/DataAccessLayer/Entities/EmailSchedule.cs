namespace DataAccessLayer.Entities;

public class EmailSchedule : BaseEntity
{
    public int Days { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
}
