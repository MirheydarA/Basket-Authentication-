using Fiorello.Entities;
using Fiorello.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fiorello.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(AccountRegisterVM model)
        {
            if (!ModelState.IsValid) return View();

            var user = new User
            {
                Email = model.Email,
                UserName = model.Username,
                Country = model.Country,
                Fullname = model.Fullname,
                PhoneNumber = model.PhoneNumber,
            };

            var result = _userManager.CreateAsync(user, model.Password).Result;
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(AccountLoginVm model)
        {
            if (!ModelState.IsValid) return View();

            var user = _userManager.FindByEmailAsync(model.Email).Result;
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email or Password is incorrect");
                return View();
            }

            var result = _signInManager.PasswordSignInAsync(user, model.Password, false, false).Result;
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email or Password is incorrect");
                return View();
            }

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }


            return RedirectToAction(nameof(Index), "home");
        }
    }
}
