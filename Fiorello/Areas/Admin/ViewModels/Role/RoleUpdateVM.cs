using System.ComponentModel.DataAnnotations;

namespace Fiorello.Areas.Admin.ViewModels.Role
{
    public class RoleUpdateVM
    {
        [Required]
        public string Name { get; set; }
    }
}
