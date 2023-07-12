using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Superadmin, Admin, HR")]
	public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

