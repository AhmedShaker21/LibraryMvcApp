using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.DocIO.DLS;
using Syncfusion.Pdf;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;

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

        //public async Task<IActionResult> Details(int id, int? folderId)
        //{
        //    var book = await _context.Books.FindAsync(id);
        //    if (book == null)
        //        return NotFound();

        //    ViewBag.FolderId = folderId;

        //    // detect if Word
        //    string fileType = book.FileType?.ToLower() ?? "";
        //    ViewBag.IsWord = fileType == "doc" || fileType == "docx";

        //    return View(book);
        //}

        public async Task<IActionResult> Details(int id, int? folderId)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            ViewBag.FolderId = folderId;

            string fileType = book.FileType?.ToLower() ?? "";

            // لو الملف Word → نحوله PDF
            if (fileType == "doc" || fileType == "docx")
            {
                string fullPath = Path.Combine(_env.WebRootPath, book.FilePath.TrimStart('/'));

                // Load Word file
                using FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                WordDocument wordDoc = new WordDocument(fileStream, Syncfusion.DocIO.FormatType.Automatic);

                // Convert Word → PDF
                DocIORenderer renderer = new DocIORenderer();
                PdfDocument pdfDocument = renderer.ConvertToPDF(wordDoc);

                // Save pdf to memory stream
                MemoryStream pdfStream = new MemoryStream();
                pdfDocument.Save(pdfStream);
                pdfStream.Position = 0;

                // Store the PDF in ViewBag as Base64
                string base64 = Convert.ToBase64String(pdfStream.ToArray());
                ViewBag.PdfBase64 = base64;
                ViewBag.IsPdf = true;

                pdfDocument.Close(true);
                renderer.Dispose();
                wordDoc.Close();

                return View(book);
            }

            // otherwise (PDF already)
            ViewBag.IsPdf = false;
            return View(book);
        }

    }
}
