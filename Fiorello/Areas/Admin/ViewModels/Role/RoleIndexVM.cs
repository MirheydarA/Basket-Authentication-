using Microsoft.AspNetCore.Identity;

namespace Fiorello.Areas.Admin.ViewModels.Role
{
	public class RoleIndexVM
	{
		public List<IdentityRole> Roles { get; set; }
	}
}
