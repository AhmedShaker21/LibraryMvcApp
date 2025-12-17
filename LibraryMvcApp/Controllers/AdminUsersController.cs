using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminUsersController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // =======================
        // LIST USERS
        // =======================
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // =======================
        // CREATE USER (GET)
        // =======================
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View();
        }

        // =======================
        // CREATE USER (POST)
        // =======================
        [HttpPost]
        public async Task<IActionResult> Create(string email, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email and Password are required");
                ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
                return View();
            }

            var user = new IdentityUser
            {
                Email = email,
                UserName = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);

                ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
                return View();
            }

            // Assign Role
            if (!string.IsNullOrEmpty(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            return RedirectToAction(nameof(Index));
        }

        // =======================
        // DELETE USER
        // =======================
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
