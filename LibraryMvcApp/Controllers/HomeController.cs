using Microsoft.AspNetCore.Mvc;

namespace LibraryMvcApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Library");
        }
    }
}
