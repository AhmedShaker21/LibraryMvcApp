using LibraryMvcApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMvcApp.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(string message, string? returnUrl = null)
        {
            var model = new ErrorViewModel
            {
                Message = message,
                ReturnUrl = returnUrl ?? Url.Action("Index", "Home")
            };

            return View(model);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
