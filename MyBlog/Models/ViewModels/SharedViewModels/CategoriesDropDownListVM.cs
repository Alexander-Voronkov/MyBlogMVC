using MyBlog.Data.Entities;

namespace MyBlog.Models.ViewModels.SharedViewModels
{
    public class CategoriesDropDownListVM
    {
        public IEnumerable<Category> Categories { get; set; } = default!;
        public int CategoryId { get; set; }
    }
}
