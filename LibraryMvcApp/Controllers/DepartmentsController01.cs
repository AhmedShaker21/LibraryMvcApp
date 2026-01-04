using LibraryMvcApp.Models;
using LibraryMvcApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMvcApp.Controllers
{
   
        [Authorize(Roles = "Admin")]
        public class DepartmentsController01 : Controller
        {
            private readonly AppDbContext _context;

            public DepartmentsController01(AppDbContext context)
            {
                _context = context;
            }

            // =========================
            // GET: Departments
            // =========================
            public async Task<IActionResult> Index()
            {
                var departments = await _context.Departments.ToListAsync();

                var result = new List<DepartmentWithLastFormVm>();

                foreach (var dept in departments)
                {
                    int lastForm = await _context.FormEntries
                        .Where(f => f.DepartmentId == dept.Id)
                        .Select(f => (int?)f.FormNumber)
                        .MaxAsync()
                        ?? dept.StartFormNumber;

                    result.Add(new DepartmentWithLastFormVm
                    {
                        Department = dept,
                        LastFormNumber = lastForm
                    });
                }

                return View(result);
            }

            // =========================
            // GET: Create
            // =========================
            public IActionResult Create()
            {
                return View();
            }

            // =========================
            // POST: Create
            // =========================
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(CreateDepartmentVm vm)
            {
                if (!ModelState.IsValid)
                    return View(vm);

                // ❌ منع تكرار رقم الإدارة
                bool exists = await _context.Departments
                    .AnyAsync(d => d.Code == vm.Code);

                if (exists)
                {
                    ModelState.AddModelError(
                        "Code",
                        "رقم الإدارة موجود بالفعل");
                    return View(vm);
                }

                var department = new Department
                {
                    Code = vm.Code,
                    StartFormNumber = vm.StartFormNumber
                };

                _context.Departments.Add(department);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }
    }
