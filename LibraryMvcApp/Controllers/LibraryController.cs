using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.DocIO.DLS;
using Syncfusion.Pdf;
using Syncfusion.DocIORenderer;
namespace LibraryMvcApp.Controllers
{
    [Authorize]
    public class LibraryController : Controller

    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LibraryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .OrderBy(b => b.Title)
                .ToListAsync();

            return View(books);
        }
        public async Task<IActionResult> Details(int id, int? folderId)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            ViewBag.FolderId = folderId;

            string fileType = book.FileType?.ToLower() ?? "";

            if (fileType == "doc" || fileType == "docx")
            {
                string fullPath = Path.Combine(_env.WebRootPath, book.FilePath.TrimStart('/'));

                using FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                WordDocument wordDoc = new WordDocument(fileStream, Syncfusion.DocIO.FormatType.Automatic);

                DocIORenderer renderer = new DocIORenderer();
                PdfDocument pdfDocument = renderer.ConvertToPDF(wordDoc);

                MemoryStream pdfStream = new MemoryStream();
                pdfDocument.Save(pdfStream);
                pdfStream.Position = 0;

                string base64 = Convert.ToBase64String(pdfStream.ToArray());
                ViewBag.PdfBase64 = base64;
                ViewBag.IsPdf = true;

                pdfDocument.Close(true);
                renderer.Dispose();
                wordDoc.Close();

                return View(book);
            }

            ViewBag.IsPdf = false;
            return View(book);
        }

    }
}
