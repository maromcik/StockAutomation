using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace DataAccessLayer.Entities;

public class HoldingSnapshotLineEntity : BaseEntity
{
    public DateTime Date { get; set; }
    public string Fund { get; set; }
    public string CompanyName { get; set; }
    public string Ticker { get; set; }
    public string Cusip { get; set; }
    public BigInteger Shares { get; set; }
    public decimal MarketValueUsd { get; set; }
    public decimal Weight { get; set; }

    public int HoldingSnapshotId { get; set; }  // Foreign key
    [ForeignKey("HoldingSnapshotId")] public HoldingSnapshot HoldingSnapshot { get; set; } = null!; // Navigation
}
