using Microsoft.AspNetCore.Mvc;
using SariouBot.Web.Models;
using System.Diagnostics;

namespace SariouBot.Web.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Ok("Hello!");
        }

    }
}