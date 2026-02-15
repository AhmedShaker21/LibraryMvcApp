//using LibraryMvcApp.Models;
//using LibraryMvcApp.Services.Interfaces;
//using LibraryMvcApp.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace LibraryMvcApp.Controllers
//{
//    [Authorize]
//    [Route("Admin/FormsRegister")]
//    public class FormsRegisterController : Controller
//    {
//        private readonly IFormRegisterService _service;
//        private readonly AppDbContext _context;
//        private readonly UserManager<IdentityUser> _userManager;

//        public FormsRegisterController(
//            IFormRegisterService service,
//            AppDbContext context,
//            UserManager<IdentityUser> userManager)
//        {
//            _service = service;
//            _context = context;
//            _userManager = userManager;
//        }

//        // =========================
//        // Helpers
//        // =========================
//        private async Task<int?> GetCurrentUserDepartmentIdAsync()
//        {
//            var userId = _userManager.GetUserId(User);

//            return await _context.UserDepartments
//                .Where(x => x.UserId == userId)
//                .Select(x => (int?)x.DepartmentId)
//                .FirstOrDefaultAsync();
//        }

//        private bool IsAdmin()
//        {
//            return User.IsInRole("Admin");
//        }

//        // =========================
//        // GET: /Admin/FormsRegister?departmentNo=50
//        // =========================
//        [HttpGet("")]
//        public async Task<IActionResult> Index([FromQuery] int departmentNo)
//        {
//            if (departmentNo <= 0)
//            {
//                TempData["ErrorMessage"] = "من فضلك اختر رقم إدارة صحيح.";
//                return RedirectToAction("AccessDenied", "Account");
//            }

//            var department = await _context.Departments
//                .FirstOrDefaultAsync(d => d.Code == departmentNo);

//            if (department == null)
//            {
//                TempData["ErrorMessage"] = "رقم الإدارة غير موجود.";
//                return RedirectToAction("AccessDenied", "Account");
//            }

//            // 🔐 Authorization by Department
//            if (!IsAdmin())
//            {
//                var userDeptId = await GetCurrentUserDepartmentIdAsync();
//                if (userDeptId == null || userDeptId != department.Id)
//                {
//                    TempData["ErrorMessage"] = "غير مسموح لك بعرض نماذج إدارة أخرى.";
//                    return RedirectToAction("AccessDenied", "Account");
//                }
//            }

//            ViewBag.DepartmentNo = departmentNo;
//            ViewBag.LastFormNumber =
//                await _service.GetLastFormNumberAsync(departmentNo);

//            var list = await _service.GetByDepartmentAsync(departmentNo);
//            return View(list);
//        }

//        // =========================
//        // GET: Create
//        // =========================
//        [HttpGet("Create")]
//        public async Task<IActionResult> Create([FromQuery] int departmentNo)
//        {
//            if (departmentNo <= 0)
//            {
//                TempData["ErrorMessage"] = "يجب اختيار إدارة قبل إضافة نموذج.";
//                return RedirectToAction("AccessDenied", "Account");
//            }

//            var department = await _context.Departments
//                .FirstOrDefaultAsync(d => d.Code == departmentNo);

//            if (department == null)
//            {
//                TempData["ErrorMessage"] = "الإدارة غير موجودة.";
//                return RedirectToAction("AccessDenied", "Account");
//            }


//            if (!IsAdmin())
//            {
//                var userDeptId = await GetCurrentUserDepartmentIdAsync();
//                if (userDeptId != department.Id)
//                {
//                    TempData["ErrorMessage"] = "غير مسموح لك بإضافة نموذج خارج إدارتك.";
//                    return RedirectToAction("AccessDenied", "Account");
//                }
//            }

//            return View(new CreateFormVm
//            {
//                DepartmentCode = departmentNo
//            });
//        }

//        // =========================
//        // POST: Create
//        // =========================
//        [HttpPost("Create")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(CreateFormVm vm)
//        {
//            if (!ModelState.IsValid)
//                return View(vm);

//            var department = await _context.Departments
//                .FirstOrDefaultAsync(d => d.Code == vm.DepartmentCode);

//            if (department == null)
//            {
//                TempData["ErrorMessage"] = "الإدارة غير موجودة.";
//                return RedirectToAction("AccessDenied", "Account");
//            }

//            if (!IsAdmin())
//            {
//                var userDeptId = await GetCurrentUserDepartmentIdAsync();
//                if (userDeptId != department.Id)
//                {
//                    TempData["ErrorMessage"] = "غير مسموح لك بإضافة نموذج خارج إدارتك.";
//                    return RedirectToAction("AccessDenied", "Account");
//                }
//            }

//            var entry = new FormEntry
//            {
//                DepartmentId = department.Id,
//                DepartmentNo = department.Code,
//                ProcedureName = vm.ProcedureName,
//                ProcedureCode = vm.ProcedureCode,
//                FormName = vm.FormName
//            };

//            await _service.AddFormAsync(entry);

//            return RedirectToAction(nameof(Index),
//                new { departmentNo = department.Code });
//        }

//        // =========================
//        // POST: Delete
//        // =========================
//        [HttpPost("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var form = await _context.FormEntries
//                .Include(f => f.Department)
//                .FirstOrDefaultAsync(f => f.Id == id);

//            if (form == null)
//            {
//                TempData["ErrorMessage"] = "النموذج غير موجود.";
//                return RedirectToAction("AccessDenied", "Account");
//            }

//            // 🔐 Authorization
//            if (!IsAdmin())
//            {
//                var userDeptId = await GetCurrentUserDepartmentIdAsync();
//                if (userDeptId != form.DepartmentId)
//                {
//                    TempData["ErrorMessage"] = "غير مسموح لك بحذف نموذج خارج إدارتك.";
//                    return RedirectToAction("AccessDenied", "Account");
//                }
//            }

//            await _service.DeleteAsync(id);
//            return RedirectToAction(nameof(Index),
//                new { departmentNo = form.DepartmentNo });
//        }

//        // =========================
//        // GET: All (Admin only)
//        // =========================
//        [HttpGet("All")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> All()
//        {
//            var list = await _service.GetAllAsync();
//            return View(list);
//        }

//        // =========================
//        // GET: Filter (AJAX)
//        // =========================
//        [HttpGet("Filter")]
//        public async Task<IActionResult> Filter(int? departmentNo)
//        {
//            if (departmentNo == null || departmentNo <= 0)
//            {
//                if (!IsAdmin())
//                {
//                    var userDeptId = await GetCurrentUserDepartmentIdAsync();
//                    var dept = await _context.Departments.FindAsync(userDeptId);
//                    return PartialView("_FormsTable",
//                        await _service.GetByDepartmentAsync(dept.Code));
//                }

//                return PartialView("_FormsTable",
//                    await _service.GetAllAsync());
//            }

//            var department = await _context.Departments
//                .FirstOrDefaultAsync(d => d.Code == departmentNo);

//            if (department == null)
//                return NotFound();

//            if (!IsAdmin())
//            {
//                var userDeptId = await GetCurrentUserDepartmentIdAsync();
//                if (userDeptId != department.Id)
//                    return PartialView("_FormsTable", new List<FormEntry>());
//            }

//            return PartialView("_FormsTable",
//                await _service.GetByDepartmentAsync(departmentNo.Value));
//        }

//        // =========================
//        // Update Review
//        // =========================
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> UpdateReview(int id, int review)
//        {
//            var form = await _context.FormEntries.FindAsync(id);

//            if (form == null)
//                return RedirectToAction("AccessDenied", "Account");

//            if (!IsAdmin())
//            {
//                var userDeptId = await GetCurrentUserDepartmentIdAsync();
//                if (userDeptId != form.DepartmentId)
//                {
//                    TempData["ErrorMessage"] = "غير مسموح لك بتعديل نموذج خارج إدارتك.";
//                    return RedirectToAction("AccessDenied", "Account");
//                }
//            }

//            form.Review = review;
//            await _context.SaveChangesAsync();

//            return RedirectToAction(nameof(Index),
//                new { departmentNo = form.DepartmentNo });
//        }
//    }
//}



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
    [Route("Admin/FormsRegister")]
    public class FormsRegisterController : Controller
    {
        private readonly IFormRegisterService _service;
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FormsRegisterController(
            IFormRegisterService service,
            AppDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _service = service;
            _context = context;
            _userManager = userManager;
        }

        // =========================
        // Helpers
        // =========================
        private async Task<int?> GetCurrentUserDepartmentIdAsync()
        {
            var userId = _userManager.GetUserId(User);

            return await _context.UserDepartments
                .Where(x => x.UserId == userId)
                .Select(x => (int?)x.DepartmentId)
                .FirstOrDefaultAsync();
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        // =========================
        // GET: /Admin/FormsRegister?departmentNo=50
        // =========================
        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] int departmentNo)
        {
            if (departmentNo <= 0)
            {
                TempData["ErrorMessage"] = "من فضلك اختر رقم إدارة صحيح.";
                return RedirectToAction("AccessDenied", "Account");
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Code == departmentNo);

            if (department == null)
            {
                TempData["ErrorMessage"] = "رقم الإدارة غير موجود.";
                return RedirectToAction("AccessDenied", "Account");
            }

            // Authorization by Department
            if (!IsAdmin())
            {
                var userDeptId = await GetCurrentUserDepartmentIdAsync();
                if (userDeptId == null || userDeptId != department.Id)
                {
                    TempData["ErrorMessage"] = "غير مسموح لك بعرض نماذج إدارة أخرى.";
                    return RedirectToAction("AccessDenied", "Account");
                }
            }

            ViewBag.DepartmentNo = departmentNo;

            ViewBag.LastFormNumber =
                await _service.GetLastFormNumberAsync(departmentNo);

            var list =
                await _service.GetByDepartmentAsync(departmentNo);

            return View(list);
        }

        // =========================
        // GET: Create
        // =========================
        //[HttpGet("Create")]
        //public async Task<IActionResult> Create([FromQuery] int departmentNo)
        //{
        //    if (departmentNo <= 0)
        //    {
        //        TempData["ErrorMessage"] =
        //            "يجب اختيار إدارة قبل إضافة نموذج.";

        //        return RedirectToAction("AccessDenied", "Account");
        //    }

        //    var department =
        //        await _context.Departments
        //        .FirstOrDefaultAsync(d => d.Code == departmentNo);

        //    if (department == null)
        //    {
        //        TempData["ErrorMessage"] = "الإدارة غير موجودة.";
        //        return RedirectToAction("AccessDenied", "Account");
        //    }

        //    if (!IsAdmin())
        //    {
        //        var userDeptId =
        //            await GetCurrentUserDepartmentIdAsync();

        //        if (userDeptId != department.Id)
        //        {
        //            TempData["ErrorMessage"] =
        //                "غير مسموح لك بإضافة نموذج خارج إدارتك.";

        //            return RedirectToAction(
        //                "AccessDenied",
        //                "Account");
        //        }
        //    }

        //    return View(new CreateFormVm
        //    {
        //        DepartmentCode = departmentNo
        //    });
        //}

        [HttpGet("Create")]
        public async Task<IActionResult> Create([FromQuery] int departmentNo)
        {
            if (departmentNo <= 0)
            {
                TempData["ErrorMessage"] =
                    "يجب اختيار إدارة قبل إضافة نموذج.";

                return RedirectToAction("AccessDenied", "Account");
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Code == departmentNo);

            if (department == null)
            {
                TempData["ErrorMessage"] = "الإدارة غير موجودة.";
                return RedirectToAction("AccessDenied", "Account");
            }

            var lastNumber =
                await _service.GetLastFormNumberAsync(departmentNo);

            return View(new CreateFormVm
            {
                DepartmentCode = departmentNo,
                LastFormNumber = lastNumber
            });
        }

        // =========================
        // POST: Create
        // =========================
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFormVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var department =
                await _context.Departments
                .FirstOrDefaultAsync(
                    d => d.Code == vm.DepartmentCode);

            if (department == null)
            {
                TempData["ErrorMessage"] =
                    "الإدارة غير موجودة.";

                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            if (!IsAdmin())
            {
                var userDeptId =
                    await GetCurrentUserDepartmentIdAsync();

                if (userDeptId != department.Id)
                {
                    TempData["ErrorMessage"] =
                        "غير مسموح لك بإضافة نموذج خارج إدارتك.";

                    return RedirectToAction(
                        "AccessDenied",
                        "Account");
                }
            }

            // ✅ التعديل هنا: إضافة FormNumber من الـ ViewModel
            var entry = new FormEntry
            {
                DepartmentId = department.Id,

                DepartmentNo = department.Code,

                ProcedureName = vm.ProcedureName,

                ProcedureCode = vm.ProcedureCode,

                FormName = vm.FormName,

                FormNumber = vm.FormNumber
            };

            await _service.AddFormAsync(entry);

            return RedirectToAction(
                nameof(Index),
                new { departmentNo = department.Code });
        }

        // =========================
        // POST: Delete
        // =========================
        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var form =
                await _context.FormEntries
                .Include(f => f.Department)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (form == null)
            {
                TempData["ErrorMessage"] =
                    "النموذج غير موجود.";

                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            if (!IsAdmin())
            {
                var userDeptId =
                    await GetCurrentUserDepartmentIdAsync();

                if (userDeptId != form.DepartmentId)
                {
                    TempData["ErrorMessage"] =
                        "غير مسموح لك بحذف نموذج خارج إدارتك.";

                    return RedirectToAction(
                        "AccessDenied",
                        "Account");
                }
            }

            await _service.DeleteAsync(id);

            return RedirectToAction(
                nameof(Index),
                new { departmentNo = form.DepartmentNo });
        }

        // =========================
        // GET: All
        // =========================
        [HttpGet("All")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> All()
        {
            var list = await _service.GetAllAsync();

            return View(list);
        }

        // =========================
        // Filter
        // =========================
        [HttpGet("Filter")]
        public async Task<IActionResult> Filter(int? departmentNo)
        {
            if (departmentNo == null || departmentNo <= 0)
            {
                if (!IsAdmin())
                {
                    var userDeptId =
                        await GetCurrentUserDepartmentIdAsync();

                    var dept =
                        await _context.Departments
                        .FindAsync(userDeptId);

                    return PartialView(
                        "_FormsTable",
                        await _service.GetByDepartmentAsync(
                            dept.Code));
                }

                return PartialView(
                    "_FormsTable",
                    await _service.GetAllAsync());
            }

            var department =
                await _context.Departments
                .FirstOrDefaultAsync(
                    d => d.Code == departmentNo);

            if (department == null)
                return NotFound();

            if (!IsAdmin())
            {
                var userDeptId =
                    await GetCurrentUserDepartmentIdAsync();

                if (userDeptId != department.Id)
                    return PartialView(
                        "_FormsTable",
                        new List<FormEntry>());
            }

            return PartialView(
                "_FormsTable",
                await _service.GetByDepartmentAsync(
                    departmentNo.Value));
        }

        // =========================
        // Update Review
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateReview(
            int id,
            int review)
        {
            var form =
                await _context.FormEntries.FindAsync(id);

            if (form == null)
                return RedirectToAction(
                    "AccessDenied",
                    "Account");

            if (!IsAdmin())
            {
                var userDeptId =
                    await GetCurrentUserDepartmentIdAsync();

                if (userDeptId != form.DepartmentId)
                {
                    TempData["ErrorMessage"] =
                        "غير مسموح لك بتعديل نموذج خارج إدارتك.";

                    return RedirectToAction(
                        "AccessDenied",
                        "Account");
                }
            }

            form.Review = review;

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Index),
                new { departmentNo = form.DepartmentNo });
        }
    }
}
