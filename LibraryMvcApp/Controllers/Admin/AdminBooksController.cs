using LibraryMvcApp.Models;
using LibraryMvcApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace LibraryMvcApp.Controllers.Admin
{
    [Authorize]
    public class AdminBooksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IFolderPermissionService _permission;

        public AdminBooksController(AppDbContext context, IWebHostEnvironment env ,     IFolderPermissionService permission)
        {
            _context = context;
            _env = env;
            _permission = permission;

        }

        // ============================================================
        // GET: Create Book
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Create(int folderId)
        {
            var folder = await _context.Folders.FindAsync(folderId);
            if (folder == null) return NotFound();

            if (!CanManage(folder))
                return Forbid();

            ViewBag.FolderId = folderId;
            return View();
        }

        // ============================================================
        // POST: Create Book
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string title,
            string? description,
            string? author,
            int folderId,
            IFormFile? file,
            IFormFile? coverImage)
        {
            var folder = await _context.Folders.FindAsync(folderId);
            if (folder == null) return NotFound();

            if (!CanManage(folder))
                return Forbid();

            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError("", "Title is required");
                ViewBag.FolderId = folderId;
                return View();
            }

            var book = new Book
            {
                Title = title,
                Description = description,
                Author = author,
                FolderId = folderId
            };

            // ---------- FILE ----------
            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", "books");
                Directory.CreateDirectory(uploads);

                var name = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var path = Path.Combine(uploads, name);

                using var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);

                book.FilePath = $"/uploads/books/{name}";
                book.FileType = Path.GetExtension(file.FileName).TrimStart('.');
            }

            // ---------- COVER ----------
            if (coverImage != null && coverImage.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", "covers");
                Directory.CreateDirectory(uploads);

                var name = Guid.NewGuid() + Path.GetExtension(coverImage.FileName);
                var path = Path.Combine(uploads, name);

                using var stream = new FileStream(path, FileMode.Create);
                await coverImage.CopyToAsync(stream);

                book.CoverImagePath = $"/uploads/covers/{name}";
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Folder", new { id = folderId });
        }

        // ============================================================
        // GET: Edit
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books
                .Include(b => b.Folder)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();
            if (!CanManage(book.Folder)) return Forbid();

            ViewBag.FolderId = book.FolderId;
            return View(book);
        }

        // ============================================================
        // POST: Edit
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            string title,
            string? description,
            string? author,
            int folderId,
            IFormFile? file,
            IFormFile? coverImage)
        {
            var book = await _context.Books
                .Include(b => b.Folder)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();
            if (!CanManage(book.Folder)) return Forbid();

            book.Title = title;
            book.Description = description;
            book.Author = author;

            // FILE
            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", "books");
                Directory.CreateDirectory(uploads);

                var name = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var path = Path.Combine(uploads, name);

                using var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);

                book.FilePath = $"/uploads/books/{name}";
                book.FileType = Path.GetExtension(file.FileName).TrimStart('.');
            }

            // COVER
            if (coverImage != null && coverImage.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", "covers");
                Directory.CreateDirectory(uploads);

                var name = Guid.NewGuid() + Path.GetExtension(coverImage.FileName);
                var path = Path.Combine(uploads, name);

                using var stream = new FileStream(path, FileMode.Create);
                await coverImage.CopyToAsync(stream);

                book.CoverImagePath = $"/uploads/covers/{name}";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Folder", new { id = book.FolderId });
        }

        // ============================================================
        // POST: Delete
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int folderId)
        {
            var book = await _context.Books
                .Include(b => b.Folder)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();
            if (!CanManage(book.Folder)) return Forbid();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Folder", new { id = folderId });
        }

        // ============================================================
        // HELPER: Permission
        // ============================================================
        private bool CanManage(Folder? folder)
        {
            if (User.IsInRole("Admin"))
                return true;

            if (folder == null)
                return false;

            if (string.IsNullOrEmpty(folder.AllowedRole))
                return false;

            return User.IsInRole(folder.AllowedRole);
        }
    }
}
