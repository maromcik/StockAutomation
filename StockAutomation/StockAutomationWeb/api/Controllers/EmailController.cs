using BusinessLayer.Facades;
using BusinessLayer.Models;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationWeb.api.Controllers;

[ApiController]
[Area("Api")]
[Route("api/[controller]")]
public class EmailController(
    IEmailService emailService,
    ISchedulerService schedulerService,
    ISendDifferencesFacade sendDifferencesFacade) : Controller
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

    [HttpGet("GetSchedule")]
    public async Task<IActionResult> GetSchedule()
    {
        var schedule = await schedulerService.GetSchedule();
        return Ok(schedule);
    }

    [HttpPost("Reschedule")]
    public async Task<IActionResult> Reschedule(EmailSchedule emailSchedule)
    {
        var res = await schedulerService.RescheduleJob(emailSchedule);
        return res.Match<IActionResult>(
            _ => Ok("Successfully Rescheduled"),
            e => BadRequest(e.Message)
        );
    }
}
