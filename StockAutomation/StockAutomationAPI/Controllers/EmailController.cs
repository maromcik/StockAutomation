using BusinessLayer.Facades;
using BusinessLayer.Models;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class EmailController(IEmailService emailService, ISendDifferencesFacade sendDifferencesFacade) : Controller
{
    [HttpPost("Send")]
    public async Task<IActionResult> SendEmail(EmailSend emailSend)
    {
        var result = await sendDifferencesFacade.ProcessDiff(emailSend);
        return result.Match<IActionResult>(
            s => Ok("Emails were successfully sent"),
            e => BadRequest(e.Message)
        );
    }

    [HttpGet("SendLatest")]
    public async Task<IActionResult> SendEmailLatest()
    {
        var result = await sendDifferencesFacade.ProcessDiffLatest();
        return result.Match<IActionResult>(
            s => Ok("Emails were successfully sent"),
            e => BadRequest(e.Message)
        );
    }


    [HttpPost("SaveSettings")]
    public async Task<IActionResult> SaveSettings(FormatSettings settings)
    {
        var result = await emailService.SaveEmailSettingsAsync(settings);
        return result.Match<IActionResult>(
            _ => Ok("Successfully Updated"),
            e => BadRequest(e.Message)
        );
    }
}
