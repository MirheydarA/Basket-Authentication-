using Fiorello.DAL;
using Fiorello.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public ProductController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var model = new ProductIndexViewModel
            {
                Products =  _appDbContext.Products.OrderByDescending(p => p.Id).Take(4).ToList()
            };


            return View(model);
        }

        public async Task<IActionResult> LoadMore(int skipRow)
        {

            bool isLast = false;
            var product = await _appDbContext.Products.OrderByDescending(p => p.Id).Skip(3 * skipRow).Take(4).ToListAsync();

            if ((3 * skipRow) + 3 >= _appDbContext.Products.Count())
            {
                isLast = true;
            }

            var model = new ProductLoadMoreViewModel
            {
                Products = product,
                Islast = isLast
            };

            return PartialView("_ProductPartial", model);

        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _appDbContext.Products.Include(p => p.Photos).Include(p => p.ProductCategory).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var model = new ProductDetailsViewModel
            {
                Id = product.Id,
                Status = product.ProductStatusType,
                Category = product.ProductCategory,
                Description = product.Description,
                Quantity = product.Quantity,
                Title = product.Title,
                Price = product.Price,
                Photos = product.Photos
            };

            return View(model);
        }
    }
}
