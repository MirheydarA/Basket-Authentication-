using Fiorello.Areas.Admin.ViewModels.Product;
using Fiorello.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Fiorello.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            //var products = _context.Products.Where(p => !p.IsDeleted).ToList();

            //var model = new ProductIndexVM
            //{
            //    Products = products
            //};
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
    }
}
