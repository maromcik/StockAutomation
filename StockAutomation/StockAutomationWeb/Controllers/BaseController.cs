using System.Diagnostics;
using BusinessLayer.Errors;
using Microsoft.AspNetCore.Mvc;
using StockAutomationWeb.Models;

namespace StockAutomationWeb.Controllers;

public class BaseController : Controller
{
    public IActionResult ErrorView(Error err)
    {
        return View("ErrorView",
            new ErrorViewModel
            {
                ErrorType = err.ErrorType, Message = err.Message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
    }
}