using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StockAutomationAPI;

[ApiController]
[Authorize]
[Route("[controller]")]
public class CoreController : Controller
{
    private readonly StockAutomationDbContext _db;


}
