using System.Diagnostics;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using StockAutomationCore.Download;
using StockAutomationWeb.Models;

namespace StockAutomationWeb.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEmailService _emailService;
    private readonly ISnapshotService<Downloader> _snapshotService;

    public HomeController(ILogger<HomeController> logger, IEmailService emailService, ISnapshotService<Downloader> snapshotService)
    {
        _logger = logger;
        _emailService = emailService;
        _snapshotService = snapshotService;
    }

    public async Task<IActionResult> Index()
    {
        var snapshots = await _snapshotService.GetSnapshotsAsync();
        var snapshotsList = new List<HoldingSnapshot>(snapshots);
        return View(snapshotsList);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
