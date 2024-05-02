using BusinessLayer.Models;
using BusinessLayer.Services;
using BusinessLayer.Facades;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationWeb.Controllers;

[Route("[controller]/[action]")]
public class EmailController(
    ILogger<EmailController> logger,
    IEmailService emailService,
    ISendDifferencesFacade sendDifferencesFacade)
    : BaseController
{
    private readonly ILogger<EmailController> _logger = logger;

    public async Task<IActionResult> Index()
    {
        var settings = await emailService.GetEmailSettings();
        return View(settings);
    }

    [HttpPost]
    public async Task<IActionResult> SendEmails()
    {
        var res = await sendDifferencesFacade.ProcessDiffLatest();
        return res.Match(
            _ => RedirectToAction("Index"),
            ErrorView);
    }


    [HttpPost]
    public async Task<IActionResult> SaveSettings(FormatSettings settings)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", settings);
        }

        var res = await emailService.SaveEmailSettingsAsync(settings);
        return res.Match(
            _ => RedirectToAction("Index"),
            ErrorView);
    }
}
