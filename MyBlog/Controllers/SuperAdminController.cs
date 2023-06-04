using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Authorization;
using MyBlog.Data.Entities;
using MyBlog.Models.ViewModels.SuperAdminViewModels;
using System.Security.Claims;

namespace MyBlog.Controllers
{
    [Authorize(Policy = MyPolicies.SuperAdminAccessOnly)]
    public class SuperAdminController : Controller
    {
        private readonly UserManager<User> _userManager;

        public SuperAdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> ManageUsersClaims()
        {
            User adminUser = await _userManager.GetUserAsync(User);
            if (adminUser is null)
            {
                return Unauthorized();
            }

            IQueryable<User> users = _userManager.Users
                .Where(u => u.EmailConfirmed == true &&
                       u.Id != adminUser.Id);

            IList<UserAccessVM> userAccessVMs = new List<UserAccessVM>();

            foreach(User user in users)
            {
                UserAccessVM userAccessVM = new UserAccessVM();

                IList<Claim> claims = await _userManager.GetClaimsAsync(user);

                if (claims.Any(c => c.Type == MyClaims.Admin))
                {
                    userAccessVM.Access = Access.Admin;
                }
                else if (claims.Any(c => c.Type == MyClaims.PostsWriter))
                {
                    userAccessVM.Access = Access.PostWriter;
                }
                else
                {
                    userAccessVM.Access = Access.None;
                }

                userAccessVM.Email = user.Email;
                userAccessVMs.Add(userAccessVM);
            }

            return View(model: userAccessVMs);
        }


        [HttpPost]
        public async Task<ActionResult<IEnumerable<User>>> ManageUsersClaims(List<UserAccessVM> userAccessVMs)
        {
            Claim adminClaim = new Claim(MyClaims.Admin, MyClaims.Admin);
            Claim postWriterClaim = new Claim(MyClaims.PostsWriter, MyClaims.PostsWriter);

            foreach (var userAccessVM in userAccessVMs)
            {
                User user = await _userManager.FindByEmailAsync(userAccessVM.Email);

                if (user is not null)
                {
                    IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
                    await _userManager.RemoveClaimsAsync(user, userClaims);

                    if (userAccessVM.Access == Access.Admin)
                    {
                        await _userManager.AddClaimAsync(user, postWriterClaim);
                        await _userManager.AddClaimAsync(user, adminClaim);
                    }
                    else if (userAccessVM.Access == Access.PostWriter)
                    {
                        await _userManager.AddClaimAsync(user, postWriterClaim);
                    }
                }
            }

            ViewBag.Message = "Users claims was managed successfuly";

            return View(userAccessVMs);
        }
    }
}
