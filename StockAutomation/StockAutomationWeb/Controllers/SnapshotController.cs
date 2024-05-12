using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationWeb.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class SnapshotController(
    ILogger<SnapshotController> logger,
    ISnapshotService snapshotService)
    : BaseController
{
    private readonly ILogger<SnapshotController> _logger = logger;

    public async Task<IActionResult> Index()
    {
        var snapshots = (await snapshotService.GetSnapshotsAsync()).Take(10).ToList();
        return View(snapshots);
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        var result = await snapshotService.DeleteSnapshotsAsync([id]);
        return result.Match(
            _ => RedirectToAction("Index", "Snapshot"),
            ErrorView
        );
    }
}