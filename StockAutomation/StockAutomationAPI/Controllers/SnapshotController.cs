using BusinessLayer.Errors;
using BusinessLayer.Models;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using StockAutomationCore.Download;

namespace StockAutomationAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SnapshotController(ISnapshotService snapshotService) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HoldingSnapshot>>> GetSnapshots()
    {
        return Ok(await snapshotService.GetSnapshotsAsync());
    }

    [HttpGet("Download")]
    public async Task<IActionResult> DownloadSnapshots()
    {
        var result = await snapshotService.DownloadSnapshotAsync();
        return result.Match<IActionResult>(
            _ => Ok("Successfully downloaded"),
            e => BadRequest(e.Message)
        );
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> DeleteSnapshots(List<int> ids)
    {
        var result = await snapshotService.DeleteSnapshotsAsync(ids);
        return result.Match<IActionResult>(
            _ => Ok("Successfully deleted"),
            e => BadRequest(e.Message)
        );
    }

    [HttpPost("Compare")]
    public async Task<IActionResult> CompareSnapshots(SnapshotCompare compare)
    {
        var result = await snapshotService.CompareSnapshotsAsync(compare.NewId, compare.OldId);
        return result.Match<IActionResult>(
            Ok,
            e =>
            {
                return e.ErrorType switch
                {
                    ErrorType.SnapshotNotFound => NotFound(e.Message),
                    ErrorType.NoSnapshotsFound => NotFound(e.Message),
                    _ => BadRequest(e.Message)
                };
            }
        );
    }
}
