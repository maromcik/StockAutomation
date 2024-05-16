using BusinessLayer.Facades;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationWeb.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController(
    ILogger<HomeController> logger,
    ISnapshotService snapshotService,
    IProcessDiffFacade processDiffFacade)
    : BaseController
{
    private readonly ILogger<HomeController> _logger = logger;

    public async Task<IActionResult> Index()
    {
        var res = await processDiffFacade.ProcessDiffLatest();
        return res.Match(
            s => View("Index", s),
            ErrorView);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
