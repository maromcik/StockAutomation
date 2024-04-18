using BusinessLayer.Models;
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
        return Ok(await _snapshotService.GetSnapshotsAsync());
    }

    [HttpGet("Download")]
    public async Task<IActionResult> DownloadSnapshots()
    {
        var result = await _snapshotService.DownloadSnapshotAsync();
        return result.Match<IActionResult>(
            _ => Ok(),
            e => BadRequest(e.Message)
        );
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> DeleteSnapshots(List<int> ids)
    {
        var result = await _snapshotService.DeleteSnapshotsAsync(ids);
        return result.Match<IActionResult>(
            _ => Ok(),
            e => BadRequest(e.Message)
        );
    }

    [HttpPost("Compare")]
    public async Task<IActionResult> CompareSnapshots(SnapshotCompare compare)
    {
        var result = await _snapshotService.CompareSnapshotsAsync(compare.NewId, compare.OldId);
        return result.Match<IActionResult>(
            Ok,
            e => BadRequest(e.Message)
        );
    }
}
