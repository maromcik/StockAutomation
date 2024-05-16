using System.ComponentModel.DataAnnotations;

namespace StockAutomationClient.Cli;

public enum Operation
{
    [Display(Name = "send latest differences")]
    SendLatest,
    [Display(Name = "send differences")] Send,

    [Display(Name = "snapshot operations")]
    Snapshot,
    [Display(Name = "email operations")] Email,
    [Display(Name = "exit")] Exit,
}

public enum SnapshotOperation
{
    [Display(Name = "print snapshots")] Print,

    [Display(Name = "download new snapshot")]
    Download,
    [Display(Name = "compare snapshots")] Compare,
    [Display(Name = "delete snapshots")] Delete,
}

public enum EmailOperation
{
    [Display(Name = "print subscribers")] Print,
    [Display(Name = "add subscriber")] Add,
    [Display(Name = "delete subscriber")] Delete,

    [Display(Name = "change attachment format")]
    ChangeFormat,
    [Display(Name = "get schedule")] GetSchedule,
    [Display(Name = "reschedule")] Reschedule,
}
