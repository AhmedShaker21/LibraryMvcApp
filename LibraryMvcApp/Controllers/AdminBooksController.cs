
using LibraryMvcApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBooksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminBooksController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //====================================================================
        // عرض الكتب حسب الفولدر (لو folderId = null → يعرض كل الكتب)
        //====================================================================
        public async Task<IActionResult> Index(int? folderId)
        {
            IQueryable<Book> booksQuery = _context.Books.AsQueryable();

            ViewBag.FolderId = folderId;

            if (folderId != null)
            {
                booksQuery = booksQuery.Where(b => b.FolderId == folderId);
            }

            var books = await booksQuery
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(books);
        }

        //====================================================================
        // GET: Create Book
        //====================================================================
        public IActionResult Create(int? folderId)
        {
            ViewBag.FolderId = folderId;
            return View();
        }


        //====================================================================
        // POST: Create Book
        //====================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    string title,
    string? description,
    string? author,
    int? folderId,
    IFormFile? file,
    IFormFile? coverImage)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError("Title", "Title is required");
                ViewBag.FolderId = folderId;
                return View();
            }
            if (folderId == 0)
                folderId = null;

            if (folderId != null)
            {
                bool folderExists = await _context.Folders.AnyAsync(f => f.Id == folderId);
                if (!folderExists)
                    folderId = null;
            }
            var book = new Book
            {
                Title = title,
                Description = description,
                Author = author,
                FolderId = folderId
            };

            //------------------------ UPLOAD FILE --------------------------
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "books");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                book.FilePath = $"/uploads/books/{fileName}";
                book.FileType = Path.GetExtension(file.FileName).TrimStart('.');
            }

            //------------------------ UPLOAD COVER --------------------------
            if (coverImage != null && coverImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "covers");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(coverImage.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await coverImage.CopyToAsync(stream);

                book.CoverImagePath = $"/uploads/covers/{fileName}";
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(
                "Index",
                "Folder",
                new { id = folderId }
            );


        }

        //====================================================================
        // GET: Edit Book
        //====================================================================
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            ViewBag.FolderId = book.FolderId;
            return View(book);
        }

        //====================================================================
        // POST: Edit Book
        //====================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            string title,
            string? description,
            string? author,
            int? folderId,
            IFormFile? file,
            IFormFile? coverImage)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError("Title", "Title is required");
                ViewBag.FolderId = folderId;
                return View(book);
            }

            book.Title = title;
            book.Description = description;
            book.Author = author;
            book.FolderId = folderId;

            //------------------------ UPDATE FILE --------------------------
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "books");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                book.FilePath = $"/uploads/books/{fileName}";
                book.FileType = Path.GetExtension(file.FileName).TrimStart('.');
            }

            //------------------------ UPDATE COVER --------------------------
            if (coverImage != null && coverImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "covers");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(coverImage.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await coverImage.CopyToAsync(stream);

                book.CoverImagePath = $"/uploads/covers/{fileName}";
            }

            await _context.SaveChangesAsync();
            int? redirectFolderId = book.FolderId;

            return RedirectToAction(
                "Index",
                "Folder",
                new { id = redirectFolderId }
            );


        }

        //====================================================================
        // GET: Delete Book
        //====================================================================
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            ViewBag.FolderId = book.FolderId;
            return View(book);
        }

        //====================================================================
        // POST: Delete Book
        //====================================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            int? folderId = book.FolderId;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(
                "Index",
                "Folder",
                new { id = folderId }
            );

        }
    }
}
