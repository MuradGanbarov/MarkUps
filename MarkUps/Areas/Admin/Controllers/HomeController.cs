using MarkUps.DAL;
using Microsoft.AspNetCore.Mvc;

namespace MarkUps.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
