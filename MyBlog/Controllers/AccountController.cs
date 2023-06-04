using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Data.Entities;
using MyBlog.Models.ViewModels;
using MyBlog.Services;

namespace MyBlog.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmailService _emailService;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(
            IEmailService emailService, 
            UserManager<User> userManager, 
            SignInManager<User> signInManager)
        {
            _emailService = emailService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid == true)
            {
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.Email,
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    string callbackUrl = Url.Action(
                        "ConfirmEmail", // (!)
                        "Account",
                        new
                        {
                            userId = user.Id,
                            code = code
                        },
                        HttpContext.Request.Scheme)!;

                    await _emailService.SendEmailAsync(
                        from: "MyBlog",
                        to: model.Email,
                        "Confirm your account",
                        html: $"To confirm registration <a href='{callbackUrl}'>follow the link</a>");

                    return View("ConfirmRegistration"); // (!)
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return View("Error");
            }

            User user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return View("Error");
            }

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return RedirectToAction("ConfirmedAccount", new { userName = user.Email });
            }
            else
            {
                return View("Error");
            }
        }


        [HttpGet]
        public IActionResult ConfirmedAccount(string userName)
        {
            return View("ConfirmedAccount", userName);
        }



        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginVM { ReturnUrl = returnUrl });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid == true)
            {
                User? user = await _userManager.FindByEmailAsync(model.Email);

                if (user is not null)
                {
                    if (await _userManager.IsEmailConfirmedAsync(user) == false)
                    {
                        ModelState.AddModelError("", "Your email was not confirmed");
                        return View(model);
                    }
                }

                if (user is null)
                {
                    return View("Error");
                }

                var signInResult = await _signInManager.PasswordSignInAsync(
                    userName: model.Email,
                    password: model.Password,
                    isPersistent: model.RememberMe,
                    lockoutOnFailure: false);

                if (signInResult.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) &&
                        Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login or password");
                }
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
