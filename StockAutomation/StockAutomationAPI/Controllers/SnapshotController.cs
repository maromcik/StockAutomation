using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SnapshotController : Controller
{
    private readonly ISnapshotService _snapshotService;

    public SnapshotController(ISnapshotService snapshotService)
    {
        _snapshotService = snapshotService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Snapshot>>> GetSnapshots()
    {
        try
        {
            return Ok(await _snapshotService.GetSnapshotsAsync());
        }
        catch (Exception e)
        {
            // TODO handle exceptions
            return NotFound();
        }
    }
}
