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

    [HttpGet("Download")]
    public async Task<ActionResult> DownloadSnapshots()
    {
        try
        {
            await _snapshotService.DownloadSnapshotAsync();
            return Ok();
        }
        catch (Exception e)
        {
            // TODO handle exceptions
            Console.WriteLine(e);
            return NotFound(e);
        }
    }

    [HttpPost("Delete")]
    public async Task<ActionResult> DeleteSnapshots(List<int> ids)
    {
        try
        {
            await _snapshotService.DeleteSnapshotsAsync(ids);
            return Ok();
        }
        catch (Exception e)
        {
            // TODO handle exceptions
            Console.WriteLine(e);
            return NotFound(e);
        }
    }

    [HttpPost("Compare")]
    public async Task<ActionResult<string>> CompareSnapshots(SnapshotCompare compare)
    {
        try
        {
            var diff = await _snapshotService.CompareSnapshotsAsync(compare.NewId, compare.OldId);
            Console.WriteLine(diff);
            return Ok(diff);
        }
        catch (Exception e)
        {
            // TODO handle exceptions
            return NotFound();
        }
    }
}
