using System.ComponentModel.DataAnnotations;

namespace Fiorello.Areas.Admin.ViewModels.Role
{
	public class RoleCreateVM
	{
        [Required]
        public string Name { get; set; }
    }
}
