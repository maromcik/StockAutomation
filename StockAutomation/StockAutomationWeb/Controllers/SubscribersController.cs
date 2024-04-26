using System.Diagnostics;
using BusinessLayer.Models;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using StockAutomationWeb.Models;

namespace StockAutomationWeb.Controllers;

[Route("[controller]/[action]")]
public class SubscribersController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEmailService _emailService;

    public SubscribersController(ILogger<HomeController> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }
    
    [HttpGet("{page:int}")]
    public async Task<IActionResult> Index(int? page = 1)
    {
        var paginationSetting = new PaginationSettings(10, page ?? 1);
        var subscribers = await _emailService.GetSearchSubscribersAsync(paginationSetting, null);
        return View(subscribers);
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _emailService.DeleteSubscribersAsync(new List<int> { id });
        if (result.IsOk)
        {
            return RedirectToAction("Index", "Subscribers", new { id = 1 });
        }
        return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
    
    public async Task<IActionResult> AddSubscriber(SubscriberCreate model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }
        
        var result = await _emailService.CreateSubscriber(model);
        if (result.IsOk)
        {
            return RedirectToAction("Index", "Subscribers", new { id = 1 });
        }
        
        return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
    
}