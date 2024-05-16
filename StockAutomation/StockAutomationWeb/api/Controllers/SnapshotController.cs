using BusinessLayer.Errors;
using BusinessLayer.Models;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationWeb.api.Controllers;

[ApiController]
[Area("Api")]
[Route("api/[controller]")]
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
        var diff = await snapshotService.CompareSnapshotsAsync(compare.NewId, compare.OldId);
        if (!diff.IsOk)
            return diff.Error.ErrorType switch
            {
                ErrorType.SnapshotNotFound => NotFound(diff.Error.Message),
                ErrorType.SnapshotsNotFound => NotFound(diff.Error.Message),
                _ => BadRequest(diff.Error.Message)
            };
        var output = await snapshotService.FormatDiff(diff.Value, OutputFormat.Text);
        return Ok(output.body);

    }
}
