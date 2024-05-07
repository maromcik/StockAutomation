using BusinessLayer.Models;
using DataAccessLayer.Entities;

namespace StockAutomationWeb.Models;


public class EmailViewModel
{
    public required FormatSettings Settings { get; set; }
    public required EmailSchedule Schedule { get; set; }
}
