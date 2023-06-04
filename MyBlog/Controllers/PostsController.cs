using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBlog.Authorization;
using MyBlog.Data;
using MyBlog.Data.Entities;
using MyBlog.Extentions;
using MyBlog.Models.ViewModels.NavigationViewModels;
using MyBlog.Models.ViewModels.PostsViewModels;

namespace MyBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationContext _context;

        public PostsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index(
            int categoryid,
            string? search,
            SortState sortOrder = SortState.TitleAsc,
            int page = 1)
        {
            int pageSize = 3;

            IQueryable<Post> posts = _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .AsNoTracking<Post>();

            // filter...
            if (categoryid != 0)
            {
                posts = posts.Where(p => p.Category!.Id == categoryid);
            }

            if (!string.IsNullOrEmpty(search))
            {
                posts = posts.Where(p => p.Title.Contains(search));
            }


            // sort
            switch (sortOrder)
            {
                case SortState.TitleDesc:
                    posts = posts.OrderByDescending(p => p.Title);
                    break;

                case SortState.DescriptionAsc:
                    posts = posts.OrderBy(p => p.Description);
                    break;
                case SortState.DescriptionDesc:
                    posts = posts.OrderByDescending(p => p.Description);
                    break;

                case SortState.CategoryAsc:
                    posts = posts.OrderBy(p => p.Category!.Name);
                    break;
                case SortState.CategoryDesc:
                    posts = posts.OrderByDescending(p => p.Category!.Name);
                    break;

                case SortState.CreatedAsc:
                    posts = posts.OrderBy(p => p.Created);
                    break;
                case SortState.CreatedDesc:
                    posts = posts.OrderByDescending(p => p.Created);
                    break;

                default:
                    posts = posts.OrderBy(p => p.Title);
                    break;
            }


            // pagination
            int itemsCount = await posts.CountAsync();

            var items = await posts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 1, 2, 3, 4, 5, 6, 7, 8, 9, 10

            //IQueryable<Category> categories = _context.Categories;
            List<Category> categories = await _context.Categories.ToListAsync();

            PostsVM postsVM = new PostsVM(
                items,
                new FilterVM(categories, categoryid, search),
                new SortVM(sortOrder),
                new PageVM(page, itemsCount, pageSize));

            return View(postsVM);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            Post? post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Comments)!
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post is null)
            {
                return NotFound();
            }

            HttpContext.Session.Set<Post>("LastViewedPosts" + post.Id, post);

            DetailsPostVM vM = new DetailsPostVM
            {
                Post = post
            };

            return View(vM);
        }

        // GET: Posts/Create

        [Authorize(Policy = MyPolicies.PostsWriterAndAboveAccess)]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyPolicies.PostsWriterAndAboveAccess)]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Body,Created,MainPostImage,IsDeleted,CategoryId,UserId")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", post.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Policy = MyPolicies.PostsWriterAndAboveAccess)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", post.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyPolicies.PostsWriterAndAboveAccess)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Body,Created,MainPostImage,IsDeleted,CategoryId,UserId")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", post.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Policy = MyPolicies.AdminAndAboveAccess)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyPolicies.AdminAndAboveAccess)]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ApplicationContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return _context.Posts.Any(e => e.Id == id);
        }
    }
}
