using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Fiorello.Areas.Admin.ViewModels.User
{
    public class UserUpdateVM
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

        [DataType(DataType.Password)]
        public string? Password { get; set; }
         [Compare("Password")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        public List<SelectListItem>? Roles { get; set; }
        public List<string> RolesIds { get; set; }
    }
}
