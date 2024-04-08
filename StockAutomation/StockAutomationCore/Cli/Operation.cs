using System.ComponentModel.DataAnnotations;

namespace StockAutomationCore.Cli;

public enum Operation
{
    [Display(Name = "file operations")] File,
    [Display(Name = "email operations")] Email,
    [Display(Name = "snapshot directory operations")] SnapshotDir,
    [Display(Name = "exit")] Exit,
}

public enum FileOperation
{
    [Display(Name = "print files")] Print,
    [Display(Name = "download new file")] Download,
    [Display(Name = "compare files")] Compare,
    [Display(Name = "delete files")] Delete,
}

public enum SnapshotDirOperation
{
    [Display(Name = "print snapshot directory")]
    Print,

    [Display(Name = "change snapshot directory")]
    Change,
}

public enum EmailOperation
{
    [Display(Name = "send differences")] Send,
    [Display(Name = "print subscribers")] Print,
    [Display(Name = "add subscriber")] Add,
    [Display(Name = "delete subscriber")] Delete,
}