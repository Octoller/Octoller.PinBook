using Microsoft.AspNetCore.Mvc;

namespace Octoller.PinBook.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
