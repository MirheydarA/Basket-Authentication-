using Fiorello.Areas.Admin.ViewModels.ProductCategory;
using Fiorello.DAL;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fiorello.Areas.Admin.Controllers
{
        [Area("Admin")]
    public class ProductCategoryController : Controller
    {
        private readonly AppDbContext _context;

        public ProductCategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var productCategories = _context.ProductCategories.OrderByDescending(pc =>  pc.Id).ToList();
            var model = new ProductCategoryIndexVM
            {
                ProductCategories = productCategories
            };
            return View(model);
        }

        #region Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductCategoryCreateVM model)
        {

            if (!ModelState.IsValid) return View();

            var productCategory = _context.ProductCategories.FirstOrDefault(pc => pc.Name.Trim().ToLower() == model.Name.Trim().ToLower());
            if (productCategory is not null)
            {
                ModelState.AddModelError("Name", "Bu adda kateqoriya movcuddur");
                return View();
            }

            productCategory = new ProductCategory
            {
                Name = model.Name,
                CreatedAt = DateTime.Now
            };

            _context.ProductCategories.Add(productCategory);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        #endregion

        #region Delete

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var  productCategory = _context.ProductCategories.FirstOrDefault(x => x.Id == id);
            if (productCategory is null) return NotFound();
            
            productCategory.IsDeleted = true;
            productCategory.DeletedAt = DateTime.Now;
            _context.SaveChanges();

            return Ok();
        }
        #endregion

        [HttpGet]
        public IActionResult Update(int id)
        {
            var productCategory = _context.ProductCategories.FirstOrDefault(pc => pc.Id == id );
            if (productCategory is null) return NotFound();

            var model = new ProductCategoryUpdateVM
            { 
                Name = productCategory.Name
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Update(int id, ProductCategoryUpdateVM model)
        {
            if (!ModelState.IsValid) return View();

            var productCategory = _context.ProductCategories.FirstOrDefault(pc => pc.Name.Trim().ToLower() == model.Name.Trim().ToLower() && pc.Id != id);     //////////

            if (productCategory is not null)
            {
                ModelState.AddModelError("Name", "Bu adda kateqoriya movcuddur");
                return View();
            }

             productCategory = _context.ProductCategories.FirstOrDefault(pc => pc.Id == id && !pc.IsDeleted);

            if (productCategory is null) return NotFound();


           
            productCategory.Name = model.Name;
            productCategory.ModifiedAt = DateTime.Now;

            _context.ProductCategories.Update(productCategory);
            _context.SaveChanges();

            return RedirectToAction("index");                                       
        }
    }
}
