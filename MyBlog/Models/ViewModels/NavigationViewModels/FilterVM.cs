using Microsoft.AspNetCore.Mvc.Rendering;
using MyBlog.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.Models.ViewModels.NavigationViewModels
{
    public class FilterVM
    {
        [Display(Name = "Categories")]
        public SelectList CategoriesSL { get; set; } = default!;
        public int CategoryId { get; set; }
        public string? Search { get; set; }


        public FilterVM(List<Category> categories, int categoryId, string? search)
        {
            categories.Insert(0, new Category { Name = "All", Id = 0 });
            CategoriesSL = new SelectList(categories, "Id", "Name", categoryId);

            CategoryId = categoryId;
            Search = search;
        }
    }
}
