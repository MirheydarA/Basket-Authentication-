using Fiorello.Entities;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace Fiorello.Areas.Admin.ViewModels.User
{
	public class UserIndexVM
	{
        public UserIndexVM()
        {
            Users = new List<UserVM>();
        }
        public List<UserVM> Users { get; set; }
	}
}
