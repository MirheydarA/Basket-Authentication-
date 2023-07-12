using System.ComponentModel.DataAnnotations;

namespace Fiorello.Areas.Admin.ViewModels.Account
{
	public class AccountLoginVM
	{
		[Required]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }
		public string? ReturnUrl { get; set; } 

	}
}
