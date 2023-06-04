using Microsoft.AspNetCore.Mvc;
using MyBlog.Data.Entities;
using MyBlog.Extentions;

namespace MyBlog.Components
{
    public class LastViewedPostsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            List<Post> sessionPosts = new List<Post>();

            foreach (string key in HttpContext.Session.Keys.Where(k => k.Contains("LastViewedPosts")))
            {
                sessionPosts.Add(HttpContext.Session.Get<Post>(key)!);
            }

            return View(sessionPosts);
        }
    }
}
