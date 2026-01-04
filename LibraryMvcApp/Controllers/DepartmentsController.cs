using LibraryMvcApp.Services.Interfaces;
using LibraryMvcApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMvcApp.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IFormRegisterService _service;

        public DepartmentsController(
            AppDbContext context,
            IFormRegisterService service)
        {
            _context = context;
            _service = service;
        }

        // 📌 عرض كل الإدارات
        public async Task<IActionResult> Index()
        {
            try
            {
                var departments = await _context.Departments
                    .OrderBy(d => d.Code)
                    .ToListAsync();

                var result = new List<DepartmentWithLastFormVm>();

                foreach (var dept in departments)
                {
                    int lastForm;

                    try
                    {
                        // ✅ نستخدم Code مش Id
                        lastForm = await _service
                            .GetLastFormNumberAsync(dept.Code);
                    }
                    catch
                    {
                        // 🔒 في حالة مشكلة في إدارة واحدة
                        lastForm = dept.StartFormNumber;
                    }

                    result.Add(new DepartmentWithLastFormVm
                    {
                        Department = dept,
                        LastFormNumber = lastForm
                    });
                }

                return View(result);
            }
            catch (Exception ex)
            {
                // 🔴 أي مشكلة عامة تودّي على Error View
                return RedirectToAction(
                    "Index",
                    "Error",
                    new { message = ex.Message });
            }
        }
    }
}
