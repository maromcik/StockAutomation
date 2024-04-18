using BusinessLayer.Models;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class EmailController(IEmailService emailService) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Subscriber>>> GetSubscribers()
    {
        return Ok(await emailService.GetSubscribersAsync());
    }


    [HttpPost("Delete")]
    public async Task<IActionResult> DeleteSubscribers(List<int> ids)
    {
        var result = await emailService.DeleteSSubscribersAsync(ids);
        return result.Match<IActionResult>(
            _ => Ok(),
            e => BadRequest(e.Message)
        );
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubscribers(SubscriberCreate subscriberCreate)
    {
        var result = await emailService.CreateSubscriber(subscriberCreate);
        return result.Match<IActionResult>(
            _ => Ok(),
            e => BadRequest(e.Message)
        );
    }
}
