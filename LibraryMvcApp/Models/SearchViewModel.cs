namespace LibraryMvcApp.Models
{
    public class SearchViewModel
    {
        public List<Folder> Folders { get; set; } = new();
        public List<Book> Books { get; set; } = new();
    }
}
