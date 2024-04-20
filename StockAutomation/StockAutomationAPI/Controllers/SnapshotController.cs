using BusinessLayer.Models;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SnapshotController(ISnapshotService snapshotService) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Snapshot>>> GetSnapshots()
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
            e => BadRequest(e.Message)
        );
    }
}
