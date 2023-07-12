using Fiorello.Entities;
using Fiorello.Enum;
using Fiorello.Utilities.EmailSender;
using Fiorello.Utilities.EmailSender.Abstract;
using Fiorello.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fiorello.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
			_roleManager = roleManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountRegisterVM model)
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

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new string[] { user.Email }, "Confirmation email link", confirmationLink);
            _emailSender.SendEmail(message);

            await _userManager.AddToRoleAsync(user, UserRoles.User.ToString());

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View("Error");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async  Task<IActionResult> Login(AccountLoginVm model)
        {
            if (!ModelState.IsValid) return View();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email or Password is incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email or Password is incorrect");
                return View();
            }

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
