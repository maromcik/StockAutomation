using BusinessLayer.Facades;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationWeb.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController(
    ILogger<HomeController> logger,
    ISnapshotService snapshotService,
    ISendDifferencesFacade sendDifferencesFacade)
    : BaseController
{
    private readonly ILogger<HomeController> _logger = logger;

    public async Task<IActionResult> Index()
    {
        var diff_body = await sendDifferencesFacade.ProcessLatestDiff();
        if (!diff_body.IsOk)
        {
            return View("Index", diff_body.Error.Message);
        }

        return View("Index", diff_body.Value);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
