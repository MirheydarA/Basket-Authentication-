using Fiorello.Areas.Admin.ViewModels.Product;
using Fiorello.Areas.Admin.ViewModels.ProductCategory;
using Fiorello.DAL;
using Fiorello.Enum;
using Fiorello.Models;
using Fiorello.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFIleService _fileService;

        public ProductController(AppDbContext context, IWebHostEnvironment webHostEnvironment, IFIleService fileService)
        {
            _context = context; 
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ProductIndexVM model)
        {
            var products = FilterByTitle(model.Title);
            products = FilterByPrice(products, model.MinPrice, model.MaxPrice);
            products = FilterByCategory(products, model.CategoriesIds);
            products = FilterByCreatedAt(products, model.CreatedAtEnd, model.CreatedAtStart);

            //var products = _context.Products.OrderByDescending(pc => !pc.IsDeleted).ToList();
            model = new ProductIndexVM
            {
                Products = products.ToList(),
                Categories = await _context.ProductCategories.Where(pc => !pc.IsDeleted).Select(pc => new SelectListItem
                {
                    Text = pc.Name,
                    Value = pc.Id.ToString(),
                }).ToListAsync()
            };
            
            return View(model);
        }

        #region FilterMethods

        private IQueryable<Product> FilterByTitle(string? Title)
        {
            var products = !string.IsNullOrEmpty(Title) ? _context.Products.Include(p => p.ProductCategory).Where(p => p.Title.Contains(Title)) : _context.Products.Include(p => p.ProductCategory).Where(p => !p.IsDeleted);
            return products;    
        }

        private IQueryable<Product> FilterByPrice(IQueryable<Product> products, decimal? minPrice, decimal? maxPrice)
        {
            return products.Where(p => minPrice != null ? p.Price >= minPrice : true && maxPrice != null ? p.Price <= maxPrice : true);
        }

        private IQueryable<Product> FilterByCategory(IQueryable<Product> products, List<int> categoriesIds)
        {
            return products.Where(p => categoriesIds.Count == 0 ? true : categoriesIds.Contains(p.ProductCategoryId));
        }

        private IQueryable<Product> FilterByCreatedAt(IQueryable<Product> products, DateTime? createdAtStart, DateTime? createdAtEnd)
        {
            return products.Where(p => createdAtStart != null ? p.CreatedAt.Date >= createdAtStart.Value.Date : true && createdAtEnd != null ? p.CreatedAt.Date <= createdAtEnd.Value.Date : true);
        }

        #endregion

        #region Create

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ProductCreateVM
            {
                ProductCategories = _context.ProductCategories.Where(pc => !pc.IsDeleted).Select(pc => new SelectListItem
                {
                    Text = pc.Name,
                    Value = pc.Id.ToString(),
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(ProductCreateVM model)
        {
            model.ProductCategories = _context.ProductCategories.Where(pc => !pc.IsDeleted).Select(pc => new SelectListItem
            {
                Text = pc.Name,
                Value = pc.Id.ToString(),
            }).ToList();

            if (!ModelState.IsValid) return View(model);

            if (!_fileService.IsImage(model.Photo))
            {
                ModelState.AddModelError("Photo", "Fayl sekil formatinda deyil");
                return View(model);
            }

            if (!_fileService.IsBiggerThanSize(model.Photo, 900))
            {
                ModelState.AddModelError("Photo", "Sekilin olcusu 100kb dan boyukdur");
                return View(model);
            }

            foreach (var photo in model.Photos)
            {
                if (!_fileService.IsImage(photo))
                {
                    ModelState.AddModelError("Photo", "Fayl sekil formatinda deyil");
                    return View(model);
                }

                if (!_fileService.IsBiggerThanSize(photo, 900))
                {
                    ModelState.AddModelError("Photo", "Sekilin olcusu 100kb dan boyukdur");
                    return View(model);
                }
            }



            Product product = new Product
            {
                Name = model.Name,
                Title = model.Title,
                Description = model.Description,
                AdditionalInfo = model.AdditionalInfo,
                Quantity = model.Quantity,
                Price = model.Price,
                PhotoName = _fileService.Upload(model.Photo),
                ProductStatusType = model.ProductStatusType,
                ProductCategoryId = model.ProductCategoryId,
                CreatedAt = DateTime.Now
            };
            _context.Products.Add(product);

            foreach (var photo in model.Photos)
            {
                var productPhotos = new ProductPhoto
                {
                    Name = _fileService.Upload(photo),
                    Product = product,
                };
                _context.ProductPhotos.Add(productPhotos);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == id);
            if (product is null) return NotFound();

            product.IsDeleted = true;
            product.DeletedAt = DateTime.Now;
            _context.SaveChanges();

            return Ok();
        }
        #endregion

        #region Update

        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = _context.Products.Include(x=> x.Photos).FirstOrDefault(pc => pc.Id == id);
            if (product is null) return NotFound();

            var model = new ProductUpdateVM
            {
                ProductCategories = _context.ProductCategories.Where(pc => !pc.IsDeleted).Select(pc => new SelectListItem
                {
                    Text = pc.Name,
                    Value = pc.Id.ToString(),
                }).ToList(),

                Name = product.Name,
                Title = product.Title,
                Description = product.Description,
                AdditionalInfo = product.AdditionalInfo,
                PhotoName = product.PhotoName,
                Price = product.Price,
                ProductStatusType = product.ProductStatusType,
                ProductCategoryId = product.ProductCategoryId

                
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(int id, ProductUpdateVM model)
        {
            model.ProductCategories = _context.ProductCategories.Where(pc => !pc.IsDeleted).Select(pc => new SelectListItem
            {
                Text=pc.Name,                                                                                               ////////////////
                Value = pc.Id.ToString(),
            }).ToList();   

            if (!ModelState.IsValid) return View();

            var product = _context.Products.Find(id);

            if (model.Photo is not null)
            {
                if (_fileService.IsImage(model.Photo))
                {
                    ModelState.AddModelError("Photo", "Seklin olcusu 100kb dan boyukdur");
                    return View();
                }

                if (_fileService.IsBiggerThanSize(model.Photo, 200))
                {
                    ModelState.AddModelError("Photo", "Sekilin olcusu 100kb dan boyukdur");
                    return View();
                }

                foreach (var photo in model.Photos)
                {
                    if (!_fileService.IsImage(photo))
                    {
                        ModelState.AddModelError("Photo", "Seklin olcusu 100kb dan boyukdur");
                        return View();
                    }

                    if (!_fileService.IsBiggerThanSize(photo, 200))
                    {
                        ModelState.AddModelError("Photo", "Sekilin olcusu 100kb dan boyukdur");
                        return View();
                    }
                } 


                _fileService.Delete(product.PhotoName);
                product.PhotoName = _fileService.Upload(model.Photo);
            }

            product = _context.Products.FirstOrDefault(p => p.Name.Trim().ToLower() == model.Name.Trim().ToLower() && !p.IsDeleted && p.Id != id);     //////////
            
            if (product is not null)
            {
                ModelState.AddModelError("Name", "Bu adda kateqoriya movcuddur");
                return View(model);
            }

             product = _context.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);

            if (product is null) return NotFound();

            var productCategory = _context.ProductCategories.FirstOrDefault(pc => pc.Id == model.ProductCategoryId && !pc.IsDeleted);

            if (productCategory is null)
            {
                ModelState.AddModelError("ProductCategoryId", "Bele kateqoriya movcud deyil");
                return View(model);
            }


            product.Name = model.Name;
            product.Title = model.Title;
            product.Description = model.Description;
            product.AdditionalInfo = model.AdditionalInfo;
            product.Price = model.Price;
            product.ProductStatusType = model.ProductStatusType;
            product.ProductCategoryId = model.ProductCategoryId;
            product.ModifiedAt = DateTime.Now;
            

            _context.Products.Update(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Details

        [HttpGet]
        public IActionResult Details(int id)
        {
            var product = _context.Products.Include(p => p.ProductCategory).FirstOrDefault(p => p.Id == id );

            if (product is null) return NotFound();

            var model = new ProductDetailsVM
            {
                Name = product.Name,
                Title = product.Title,
                Description = product.Description,
                AdditionalInfo = product.AdditionalInfo,
                Price = product.Price,
                PhotoName = product.PhotoName,
                ProductStatusType = product.ProductStatusType,
                ProductCategory = product.ProductCategory
            };

            return View(model);
        }
        #endregion


    }
}
