using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace Fiorello.Areas.Admin.ViewModels.User
{
	public class UserCreateVM
	{
        [Required]
        public string Username { get; set; }
        [Required]
        public string Fullname { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set;}

        public List<SelectListItem>? Roles { get; set; }
        public List<string> RolesIds { get; set; }

    }
}
