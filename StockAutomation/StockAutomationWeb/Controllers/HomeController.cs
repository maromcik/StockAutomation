using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationWeb.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    ISnapshotService snapshotService)
    : BaseController
{
    private readonly ILogger<HomeController> _logger = logger;

    public async Task<IActionResult> Index()
    {
        var snapshots = await snapshotService.GetSnapshotsAsync();
        var snapshotsList = new List<HoldingSnapshot>(snapshots);
        return View(snapshotsList);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await snapshotService.DeleteSnapshotsAsync([id]);
        return result.Match(
            _ => RedirectToAction("Index", "Home"),
            ErrorView
        );
    }
}