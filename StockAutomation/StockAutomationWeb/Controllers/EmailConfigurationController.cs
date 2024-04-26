using System.Diagnostics;
using BusinessLayer.Models;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using StockAutomationWeb.Models;

namespace StockAutomationWeb.Controllers;

[Route("[controller]/[action]")]
public class EmailConfigurationController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEmailService _emailService;

    public EmailConfigurationController(ILogger<HomeController> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }
    
    public async Task<IActionResult> Index()
    {
        var settings = await _emailService.GetEmailSettings();
        return View(settings);
    }

    [HttpPost]
    public async Task<IActionResult> SendEmails()
    {
        await _emailService.SendEmailAsync("html diff here");
    
        return RedirectToAction("Index");
    }
    
    
    [HttpPost]
    public async Task<IActionResult> SaveSettings(FormatSettings settings)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", settings);
        }
        
        await _emailService.SaveEmailSettingsAsync(settings);
        return RedirectToAction("Index");
    }
    
}