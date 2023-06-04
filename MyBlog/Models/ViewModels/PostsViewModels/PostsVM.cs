using MyBlog.Data.Entities;
using MyBlog.Models.ViewModels.NavigationViewModels;

namespace MyBlog.Models.ViewModels.PostsViewModels
{
    public class PostsVM
    {
        public IEnumerable<Post> Posts { get; set; } = default!;
        //public IEnumerable<Category> Categories { get; set; } = default!;
        //public int CategoryId { get; set; }

        public FilterVM? FilterVM { get; set; }
        public SortVM? SortVM { get; set; }
        public PageVM? PageVM { get; set; }

        public PostsVM(IEnumerable<Post> posts, 
            FilterVM? filterVM,
            SortVM? sortVM,
            PageVM? pageVM)
        {
            Posts = posts;
            FilterVM = filterVM;
            SortVM = sortVM;
            PageVM = pageVM;
        }
    }
}
