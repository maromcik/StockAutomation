using DataAccessLayer.Entities;

namespace StockAutomationWeb.Models;

public class SnapshotsListModel
{
    public required IEnumerable<HoldingSnapshot> Snapshots;
}