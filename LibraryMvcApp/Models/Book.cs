using System;

namespace LibraryMvcApp.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Author { get; set; }

        public string FilePath { get; set; } = string.Empty;
        public string CoverImagePath { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty;

        public int? FolderId { get; set; }
        public Folder? Folder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
