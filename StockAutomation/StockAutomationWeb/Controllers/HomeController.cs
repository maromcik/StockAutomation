using System.Diagnostics;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using StockAutomationWeb.Models;

namespace StockAutomationWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEmailService _emailService;
    private readonly ISnapshotService _snapshotService;

    public HomeController(ILogger<HomeController> logger, IEmailService emailService, ISnapshotService snapshotService)
    {
        _logger = logger;
        _emailService = emailService;
        _snapshotService = snapshotService;
    }

    public async Task<IActionResult> Index()
    {
        var snapshots = await _snapshotService.GetSnapshotsAsync();

        var snapshotsList = snapshots.ToList();
        var firstNineSnapshots = snapshotsList.ToList();

        var model = new SnapshotsListModel
        {
            Snapshots = firstNineSnapshots
        };

        return View(model);
    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}