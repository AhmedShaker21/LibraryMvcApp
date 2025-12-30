using LibraryMvcApp.Services.Interfaces;
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
            var departments = await _context.Departments.ToListAsync();

            var result = new List<dynamic>();

            foreach (var dept in departments)
            {
                int lastForm = await _service.GetLastFormNumberAsync(dept.Id);

                result.Add(new
                {
                    Department = dept,
                    LastFormNumber = lastForm
                });
            }

            return View(result);
        }
    }

}
