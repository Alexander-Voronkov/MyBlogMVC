using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Authorization;

namespace MyBlog.Controllers
{
    public class TestClaimsController : Controller
    {

        [Authorize(Policy = MyPolicies.SuperAdminAccessOnly)]
        public IActionResult SuperAdmin()
        {
            return Content(User.Identity!.Name!);
        }

        [Authorize(Policy = MyPolicies.AdminAndAboveAccess)]
        public IActionResult Admin()
        {
            return Content(User.Identity!.Name!);
        }

        [Authorize(Policy = MyPolicies.PostsWriterAndAboveAccess)]
        //[Authorize(Roles = "Role1,Role2")]
        public IActionResult PostsWriter()
        {
            return Content(User.Identity!.Name!);
        }
    }
}
