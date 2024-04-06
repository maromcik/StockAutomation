using System.ComponentModel.DataAnnotations;

namespace StockAutomationCore.Cli;

public enum Operation
{
    [Display(Name = "download new file")]
    Download,
    [Display(Name = "compare files")]
    Compare,
    [Display(Name = "send differences")]
    Send,
    [Display(Name = "snapshot directory operations")]
    SnapshotDir,
    [Display(Name = "file operations")]
    File,
    [Display(Name = "subscriber operations")]
    Subscriber,
    [Display(Name = "exit")]
    Exit,
}

public enum FileOperation
{
    [Display(Name = "print files")]
    Print,
    [Display(Name = "delete files")]
    Delete,
}


public enum SnapshotDirOperation
{
    [Display(Name = "print snapshot directory")]
    Print,
    [Display(Name = "change snapshot directory")]
    Change,
}

public enum SubscriberOperation
{
    [Display(Name = "print subscribers")]
    Print,
    [Display(Name = "add subscriber")]
    Add,
    [Display(Name = "delete subscriber")]
    Delete,
}