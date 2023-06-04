namespace MyBlog.Models.ViewModels.NavigationViewModels
{
    public class PageVM
    {
        public int PageNumber { get; }
        public int TotalPages { get; set; }

        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;

        public PageVM(int pageNumber, int count, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }
}
