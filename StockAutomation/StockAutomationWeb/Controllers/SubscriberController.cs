using BusinessLayer.Models;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationWeb.Controllers;

[Route("[controller]/[action]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class SubscriberController(ILogger<SubscriberController> logger, ISubscriberService subscriberService)
    : BaseController
{
    private readonly ILogger<SubscriberController> _logger = logger;

    [HttpGet("{page:int}")]
    public async Task<IActionResult> Index(int? page = 1)
    {
        var paginationSetting = new PaginationSettings(10, page ?? 1);
        var subscribers = await subscriberService.SearchSubscribersAsync(paginationSetting, null);
        return View(subscribers);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await subscriberService.DeleteSubscribersAsync([id]);
        return result.Match(
            _ => RedirectToAction("Index", "Subscriber", new { id = 1 }),
            ErrorView
        );
    }

    public async Task<IActionResult> Add(SubscriberCreate model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }

        var result = await subscriberService.CreateSubscriber(model);
        return result.Match(
            _ => RedirectToAction("Index", "Subscriber", new { id = 1 }),
            ErrorView
        );
    }
}
