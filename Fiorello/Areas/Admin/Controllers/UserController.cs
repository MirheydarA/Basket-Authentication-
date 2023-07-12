using Fiorello.Areas.Admin.ViewModels.User;
using Fiorello.Entities;
using Fiorello.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography.Xml;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Superadmin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new UserIndexVM();
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!await _userManager.IsInRoleAsync(user, UserRoles.Superadmin.ToString()))
                {
                    var userWithRoles = new UserVM
                    {
                        Id = user.Id,
                        Fullname = user.Fullname,
                        Email = user.Email,
                        Username = user.UserName,
                        Roles = roles.ToList()
                    };

                    model.Users.Add(userWithRoles);
                }
            }
            return View(model);
        }

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new UserCreateVM()
            {
                Roles = await _roleManager.Roles.Where(r => r.Name != UserRoles.User.ToString() && r.Name != UserRoles.Superadmin.ToString()).Select(r => new SelectListItem
                {
                    Value = r.Id,
                    Text = r.Name
                }).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateVM model)
        {
            model.Roles = await _roleManager.Roles.Where(r => r.Name != UserRoles.User.ToString() &&
                                                              r.Name != UserRoles.Superadmin.ToString())
                                                  .Select(r => new SelectListItem
                                                  {
                                                      Value = r.Id,
                                                      Text = r.Name
                                                  })
                                                   .ToListAsync();


            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user is not null)
            {
                ModelState.AddModelError("Username", "Bu adda istifadeci movcuddur");
                return View();
            }

            user = new User
            {
                UserName = model.Username,
                Country = model.Country,
                Email = model.Email,
                Fullname = model.Fullname,
                PhoneNumber = model.PhoneNumber
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

            foreach (var roleid in model.RolesIds)
            {
                var role = await _roleManager.FindByIdAsync(roleid);
                if (role is null)
                {
                    ModelState.AddModelError("RoleIds", "rol movcud deyil");
                    return View();
                }

                result = await _userManager.AddToRoleAsync(user, role.Name);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return View();
                }
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion


        #region Details
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var userWithRoles = new UserDetailsVM
            {
                UserName = user.UserName,
                Country = user.Country,
                Email = user.Email,
                FullName = user.Fullname,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToList(),
            };

            return View(userWithRoles);
        }
        #endregion


        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    throw new Exception(error.Description);
                }
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion


        #region Update
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var rolesIds = new List<string>();
            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role is null)
                {
                    throw new Exception("Rol movcud deyil");
                }

                rolesIds.Add(role.Id);
            }

            var model = new UserUpdateVM
            {
                Fullname = user.Fullname,
                Country = user.Country,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName,
                Roles = await _roleManager.Roles.Where(r => r.Name != UserRoles.User.ToString() && r.Name != UserRoles.Superadmin.ToString()).Select(r => new SelectListItem
                {
                    Value = r.Id,
                    Text = r.Name
                }).ToListAsync(),

                RolesIds = rolesIds

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id, UserUpdateVM model)
        {
            model.Roles = await _roleManager.Roles.Where(r => r.Name != UserRoles.User.ToString() &&
                                                              r.Name != UserRoles.Superadmin.ToString())
                                                  .Select(r => new SelectListItem
                                                  {
                                                      Value = r.Id,
                                                      Text = r.Name
                                                  })
                                                   .ToListAsync();

            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            user.Fullname = model.Fullname;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Country = model.Country;
            user.UserName = model.Username;
            if (model.Password != null)
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            var userRoles = new List<IdentityRole>();
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    throw new Exception("Rol movcud deyil");
                }
                userRoles.Add(role);
            }
            var removedIds = userRoles.FindAll(x => !model.RolesIds.Contains(x.Id)).ToList();

            foreach (var removedRole in removedIds)
            {
                await _userManager.RemoveFromRoleAsync(user, removedRole.Name);
            }

            foreach (var roleId in model.RolesIds)
            {
                var role = await _roleManager.FindByIdAsync(roleId);

                if (role == null)
                {
                    ModelState.AddModelError("RolesIds", "Rol movcud deyil");
                    return View();
                }

                if (!await _userManager.IsInRoleAsync(user, role.Name))
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                }

                else if (!userRoles.Any(r => r.Id == role.Id))
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

    }
}

