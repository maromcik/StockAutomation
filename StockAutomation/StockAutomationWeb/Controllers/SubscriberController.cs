using System.Diagnostics;
using BusinessLayer.Models;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using StockAutomationWeb.Models;

namespace StockAutomationWeb.Controllers;

[Route("[controller]/[action]")]
public class SubscriberController(ILogger<SubscriberController> logger, IEmailService emailService)
    : BaseController
{
    private readonly ILogger<SubscriberController> _logger = logger;

    [HttpGet("{page:int}")]
    public async Task<IActionResult> Index(int? page = 1)
    {
        var paginationSetting = new PaginationSettings(10, page ?? 1);
        var subscribers = await emailService.SearchSubscribersAsync(paginationSetting, null);
        return View(subscribers);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await emailService.DeleteSubscribersAsync([id]);
        return result.Match(
            _ => RedirectToAction("Index", "Subscriber", new { id = 1 }),
            ErrorView
        );
    }

    public async Task<IActionResult> AddSubscriber(SubscriberCreate model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }

        var result = await emailService.CreateSubscriber(model);
        return result.Match(
            _ => RedirectToAction("Index", "Subscriber", new { id = 1 }),
            ErrorView
        );
    }
}