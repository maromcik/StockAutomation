using BusinessLayer.Models;
using BusinessLayer.Services;
using BusinessLayer.Facades;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationWeb.Controllers;

[Route("[controller]/[action]")]
public class EmailController(ILogger<EmailController> logger, IEmailService emailService, ISendDifferencesFacade sendDifferencesFacade)
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
        await sendDifferencesFacade.ProcessDiffLatest();

        return RedirectToAction("Index");
    }


    [HttpPost]
    public async Task<IActionResult> SaveSettings(FormatSettings settings)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", settings);
        }

        await emailService.SaveEmailSettingsAsync(settings);
        return RedirectToAction("Index");
    }
}
