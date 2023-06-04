using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data;
using MyBlog.Data.Entities;
using MyBlog.Models.ViewModels.SharedViewModels;

namespace MyBlog.Components
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly ApplicationContext _context;

        public CategoriesViewComponent(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IQueryable<Category> categories = _context.Categories;
            int categoryId = 0;

            if (HttpContext.Request.Query.ContainsKey("categoryId"))
            {
                int.TryParse(Request.Query["categoryId"], out categoryId);
            }

            CategoriesDropDownListVM model = new()
            {
                Categories = await categories.ToListAsync(),
                CategoryId = categoryId,
            };

            return View(model);
        }
    }
}
