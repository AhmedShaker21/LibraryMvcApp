using LibraryMvcApp.Models;
using LibraryMvcApp.Services.Interfaces;
using LibraryMvcApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMvcApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/FormsRegister")]
    public class FormsRegisterController : Controller
    {
        private readonly IFormRegisterService _service;
        private readonly AppDbContext _context;

        public FormsRegisterController(
            IFormRegisterService service,
            AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        // =========================
        // GET: /Admin/FormsRegister?departmentId=1
        // =========================
        public async Task<IActionResult> Index(int departmentNo)
        {
            ViewBag.DepartmentNo = departmentNo;

            ViewBag.LastFormNumber =
                await _service.GetLastFormNumberAsync(departmentNo);

            var list = await _service.GetByDepartmentAsync(departmentNo);

            return View(list);
        }

        // =========================
        // GET: /Admin/FormsRegister/Create
        // =========================
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateFormVm
            {
                Departments = await _context.Departments
                    .OrderBy(d => d.Code)
                    .ToListAsync()
            };

            return View(vm);
        }

        // =========================
        // POST: /Admin/FormsRegister/Create
        // =========================
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFormVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Departments = await _context.Departments.ToListAsync();
                return View(vm);
            }

            var department = await _context.Departments
                .SingleAsync(d => d.Id == vm.DepartmentId);

            var entry = new FormEntry
            {
                DepartmentId = department.Id,        // FK
                DepartmentNo = department.Code,      // 53
                ProcedureName = vm.ProcedureName,
                ProcedureCode = vm.ProcedureCode,
                FormName = vm.FormName
            };

            await _service.AddFormAsync(entry);

            return RedirectToAction(
                nameof(Index),
                new { departmentNo = department.Code }
            );
        }


        // =========================
        // POST: Delete
        // =========================
        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int departmentId)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { departmentId });
        }

        // =========================
        // GET: All Forms
        // =========================
        [HttpGet("All")]
        public async Task<IActionResult> All()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }
    }


}
