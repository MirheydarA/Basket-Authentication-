using Fiorello.Entities;
using Fiorello.Enum;
using Microsoft.AspNetCore.Identity;

namespace Fiorello.Utilities
{
    public static class Dbinitializer
	{
		public async static Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
		{
		 	await SeedRolesAsync(roleManager);
			await SeedUsersAsync(userManager);
		}

		private async static Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
		{
			foreach (var role in System.Enum.GetValues<UserRoles>())
			{
				if (!await roleManager.RoleExistsAsync(role.ToString()))
				{
				await roleManager.CreateAsync(new IdentityRole
				{
					Name = role.ToString(),
				});
				}
			}
		}

		private async static Task SeedUsersAsync(UserManager<User> userManager)
		{
			var user = await userManager.FindByNameAsync("Admin");

			if (user is null)
			{
			user = new User
			{
				UserName = "Admin",
				Fullname = "Admin",
				Email = "admin@app.com",
				Country = "Aze"
			};
			var result = await userManager.CreateAsync(user, "admin123");
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					throw new Exception(error.Description);
				}
			}

			await userManager.AddToRoleAsync(user, UserRoles.Superadmin.ToString());
			}
		}
	}
}
