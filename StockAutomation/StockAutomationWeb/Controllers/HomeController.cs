using System.Diagnostics;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using StockAutomationWeb.Models;

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
}