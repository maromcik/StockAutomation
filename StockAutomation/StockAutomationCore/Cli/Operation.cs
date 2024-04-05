using System.ComponentModel.DataAnnotations;

namespace StockAutomationCore.Cli;

public enum Operation
{
    [Display(Name = "print files")]
    PrintFiles,
    [Display(Name = "delete files")]
    DeleteFiles,
    [Display(Name = "download new file")]
    Download,
    [Display(Name = "compare files")]
    Compare,
    [Display(Name = "send differences")]
    Send,
    [Display(Name = "add email subscriber")]
    AddSubscriber,
    [Display(Name = "print working directory")]
    PrintDir,
    [Display(Name = "change working directory")]
    ChangeDir,
    [Display(Name = "exit")]
    Exit,
}