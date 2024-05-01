using BusinessLayer.Models;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SubscriberController(ISubscriberService subscriberService) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Subscriber>>> GetSubscribers()
    {
        return Ok(await subscriberService.GetSubscribersAsync());
    }


    [HttpPost("Delete")]
    public async Task<IActionResult> DeleteSubscribers(List<int> ids)
    {
        var result = await subscriberService.DeleteSubscribersAsync(ids);
        return result.Match<IActionResult>(
            _ => Ok("Successfully deleted"),
            e => BadRequest(e.Message)
        );
    }


    [HttpPost]
    public async Task<IActionResult> CreateSubscribers(SubscriberCreate subscriberCreate)
    {
        var result = await subscriberService.CreateSubscriber(subscriberCreate);
        return result.Match<IActionResult>(
            _ => Ok("Successfully created"),
            e => BadRequest(e.Message)
        );
    }

}
