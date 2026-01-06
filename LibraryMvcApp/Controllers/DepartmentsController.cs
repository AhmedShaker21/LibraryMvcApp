using LibraryMvcApp.Models;
using LibraryMvcApp.Services.Interfaces;
using LibraryMvcApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMvcApp.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IFormRegisterService _service;
        
    private readonly UserManager<IdentityUser> _userManager;

        public DepartmentsController(
            AppDbContext context,
            IFormRegisterService service,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _service = service;
            _userManager = userManager;
        }

        // 📌 عرض كل الإدارات
        //public async Task<IActionResult> Index()
        //{
        //    try
        //    {
        //        var departments = await _context.Departments
        //            .OrderBy(d => d.Code)
        //            .ToListAsync();

        //        var result = new List<DepartmentWithLastFormVm>();

        //        foreach (var dept in departments)
        //        {
        //            int lastForm;

        //            try
        //            {
        //                // ✅ نستخدم Code مش Id
        //                lastForm = await _service
        //                    .GetLastFormNumberAsync(dept.Code);
        //            }
        //            catch
        //            {
        //                // 🔒 في حالة مشكلة في إدارة واحدة
        //                lastForm = dept.StartFormNumber;
        //            }

        //            result.Add(new DepartmentWithLastFormVm
        //            {
        //                Department = dept,
        //                LastFormNumber = lastForm
        //            });
        //        }

        //        return View(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        // 🔴 أي مشكلة عامة تودّي على Error View
        //        return RedirectToAction(
        //            "Index",
        //            "Error",
        //            new { message = ex.Message });
        //    }
        //}

        public async Task<IActionResult> Index()
        {
            List<DepartmentWithLastFormVm> result;

            // 👑 Admin → يشوف الكل
            if (User.IsInRole("Admin"))
            {
                result = await _context.Departments
                    .Select(d => new DepartmentWithLastFormVm
                    {
                        Department = d,
                        LastFormNumber = _context.FormEntries
                            .Where(f => f.DepartmentId == d.Id)
                            .Max(f => (int?)f.FormNumber) ?? d.StartFormNumber
                    })
                    .ToListAsync();

                return View(result);
            }

            // 👤 User عادي → يشوف إدارته بس
            var userId = _userManager.GetUserId(User);

            var userDepartmentId = await _context.UserDepartments
                .Where(x => x.UserId == userId)
                .Select(x => x.DepartmentId)
                .FirstOrDefaultAsync();

            if (userDepartmentId == 0)
            {
                TempData["ErrorMessage"] = "لم يتم ربطك بأي إدارة.";
                return RedirectToAction("AccessDenied", "Account");
            }

            result = await _context.Departments
                .Where(d => d.Id == userDepartmentId)
                .Select(d => new DepartmentWithLastFormVm
                {
                    Department = d,
                    LastFormNumber = _context.FormEntries
                        .Where(f => f.DepartmentId == d.Id)
                        .Max(f => (int?)f.FormNumber) ?? d.StartFormNumber
                })
                .ToListAsync();

            return View(result);
        }

        // =========================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // =========================
        // POST: Departments/Create
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDepartmentVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // منع تكرار رقم الإدارة
            bool codeExists = await _context.Departments
                .AnyAsync(d => d.Code == vm.Code);

            if (codeExists)
            {
                ModelState.AddModelError("Code", "رقم الإدارة موجود بالفعل");
                return View(vm);
            }

            var department = new Department
            {
                Name = vm.Name,                
                Code = vm.Code,
                StartFormNumber = vm.StartFormNumber
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _context.Departments
                .Include(d => d.FormEntries)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
                return NotFound();

            if (department.FormEntries.Any())
            {
                TempData["Error"] = "لا يمكن حذف الإدارة لأنها تحتوي على نماذج";
                return RedirectToAction(nameof(Index));
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم حذف الإدارة بنجاح";
            return RedirectToAction(nameof(Index));
        }

    }

}
