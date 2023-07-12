namespace Fiorello.Areas.Admin.ViewModels.User
{
    public class UserDetailsVM
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Country { get; set; }
        public List<string> Roles { get; set; }

    }
}
