using LibraryMvcApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMvcApp.Controllers
{
    public class FolderController : Controller
    {
        private readonly AppDbContext _context;

        public FolderController(AppDbContext context)
        {
            _context = context;
        }

        // =======================================================
        // 📌 عرض فولدر + فولدرات فرعية + كتب + Breadcrumb
        // =======================================================
        public async Task<IActionResult> Index(int? id)
        {
            Folder folder;

            if (id == null || id == 0)
            {
                // 📁 Root (الفولدر الأساسي)
                folder = new Folder
                {
                    Id = 0,
                    Name = "Main",
                    SubFolders = await _context.Folders
                        .Where(f => f.ParentFolderId == null)
                        .ToListAsync(),
                    Books = await _context.Books
                        .Where(b => b.FolderId == null)
                        .ToListAsync()
                };

                ViewBag.Breadcrumb = new List<Folder>();  // root has no parents
            }
            else
            {
                folder = await _context.Folders
                    .Include(f => f.SubFolders)
                    .Include(f => f.Books)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (folder == null) return NotFound();

                // تحميل الـ Breadcrumb
                ViewBag.Breadcrumb = await GetBreadcrumbPath(id.Value);
            }

            return View(folder);
        }

        // =======================================================
        // 📌 إنشاء فولدر
        // =======================================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(string name, int? parentId)
        {
            if (parentId == 0)
                parentId = null; // root

            var folder = new Folder
            {
                Name = name,
                ParentFolderId = parentId
            };

            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = parentId });
        }

        // =======================================================
        // 📌 Breadcrumb Path
        // =======================================================
        private async Task<List<Folder>> GetBreadcrumbPath(int folderId)
        {
            var path = new List<Folder>();
            var current = await _context.Folders.FindAsync(folderId);

            while (current != null)
            {
                path.Add(current);

                if (current.ParentFolderId == null)
                    break;

                current = await _context.Folders
                    .FirstOrDefaultAsync(f => f.Id == current.ParentFolderId);
            }

            path.Reverse();
            return path;
        }

        // =======================================================
        // 📌 Rename Folder
        // =======================================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Rename(int id, string newName)
        {
            var folder = await _context.Folders.FindAsync(id);
            if (folder == null) return NotFound();

            folder.Name = newName;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = folder.ParentFolderId });
        }

        // =======================================================
        // 📌 Delete Folder (Recursive)
        // =======================================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var folder = await _context.Folders
                .Include(f => f.SubFolders)
                .Include(f => f.Books)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (folder == null) return NotFound();

            int? parentId = folder.ParentFolderId;

            // حذف كامل شامل لكل ما بداخل الفولدر
            await DeleteFolderRecursive(folder);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = parentId });
        }

        private async Task DeleteFolderRecursive(Folder folder)
        {
            // حذف الكتب داخل الفولدر
            _context.Books.RemoveRange(folder.Books);

            // حذف الفولدرات الفرعية بشكل Recursively
            foreach (var sub in folder.SubFolders.ToList())
            {
                var child = await _context.Folders
                    .Include(f => f.SubFolders)
                    .Include(f => f.Books)
                    .FirstAsync(f => f.Id == sub.Id);

                await DeleteFolderRecursive(child);
            }

            // حذف الفولدر نفسه
            _context.Folders.Remove(folder);
        }
    }
}
