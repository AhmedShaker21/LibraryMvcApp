using LibraryMvcApp.Models;
using LibraryMvcApp.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LibraryMvcApp.Services.Implementations
{
    public class FormRegisterService : IFormRegisterService
    {
        private readonly AppDbContext _context;

        public FormRegisterService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddFormAsync(FormEntry entry)
        {
            var department = await _context.Departments
                .SingleAsync(d => d.Id == entry.DepartmentId);

            entry.DepartmentNo = department.Code;

            int lastFormNumber = await _context.FormEntries
                .Where(x => x.DepartmentId == department.Id)
                .Select(x => (int?)x.FormNumber)
                .MaxAsync()
                ?? department.StartFormNumber;

            entry.FormNumber = lastFormNumber + 1;

            entry.FullNumber = $"ن / {entry.DepartmentNo} / {entry.FormNumber}";
            entry.CreatedAt = DateTime.Now;

            _context.FormEntries.Add(entry);
            await _context.SaveChangesAsync();
        }

        // =========================
        // GET BY DEPARTMENT
        // =========================
        public async Task<List<FormEntry>> GetByDepartmentAsync(int departmentNo)
        {
            return await _context.FormEntries
                .Where(x => x.DepartmentNo == departmentNo)
                .OrderBy(x => x.FormNumber)
                .ToListAsync();
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<FormEntry>> GetAllAsync()
        {
            return await _context.FormEntries
                .OrderBy(x => x.DepartmentNo)
                .ThenBy(x => x.FormNumber)
                .ToListAsync();
        }

        // =========================
        // LAST FORM NUMBER (KEY PART)
        // =========================
        public async Task<int> GetLastFormNumberAsync(int departmentNo)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Code == departmentNo);

            if (department == null || department.StartFormNumber == null)
                throw new Exception("Department not found or StartFormNumber not set");

            var lastFormNumber = await _context.FormEntries
                .Where(f => f.DepartmentId == department.Id)
                .Select(f => (int?)f.FormNumber)
                .MaxAsync();

            return lastFormNumber ?? department.StartFormNumber;
        }

        public async Task DeleteAsync(int id)
        {
            var entry = await _context.FormEntries.FindAsync(id);
            if (entry == null) return;

            _context.FormEntries.Remove(entry);
            await _context.SaveChangesAsync();
        }


        public async Task<Dictionary<int, int>> GetLastNumbersPerDepartmentAsync()
        {
            return await _context.FormEntries
                .GroupBy(x => x.DepartmentNo)
                .Select(g => new
                {
                    Department = g.Key,
                    LastNumber = g.Max(x => x.FormNumber)
                })
                .ToDictionaryAsync(x => x.Department, x => x.LastNumber);
        }
     

    }

}
