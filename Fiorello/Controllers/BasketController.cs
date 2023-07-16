using Fiorello.DAL;
using Fiorello.Entities;
using Fiorello.Models;
using Fiorello.ViewModels.Basket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fiorello.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Entities.User> _userManager;

        public BasketController(AppDbContext context, UserManager<Entities.User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var basket = await _context.Baskets.Include(b => b.BasketProducts).ThenInclude(bp => bp.Product).FirstOrDefaultAsync(b => b.UserId == user.Id);

            var model = new List<BasketVM>();

            if (basket is null)
                return View(model);


            foreach (var basketProduct in basket.BasketProducts)
            {
                var basketProductItem = new BasketVM
                {
                    Id = basketProduct.Id,
                    Count = basketProduct.Count,
                    PhotoName = basketProduct.Product.PhotoName,
                    StockQuantity = basketProduct.Product.Quantity,
                    Title = basketProduct.Product.Title,
                    Price = basketProduct.Product.Price
                };

                model.Add(basketProductItem);
            }

            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> AddAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var basket = await _context.Baskets.FirstOrDefaultAsync(b => b.UserId == user.Id);
            if (basket == null)
            {
                basket = new Basket
                {
                    UserId = user.Id
                };
                await _context.Baskets.AddAsync(basket);
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound("mehsul tapilmadi");

            var basketProduct = await _context.BasketProducts.FirstOrDefaultAsync(basketProduct => basketProduct.ProductId == id && basketProduct.Basket.UserId == user.Id);
            if (basketProduct == null)
            {
                basketProduct = new BasketProduct
                {
                    Basket = basket,
                    ProductId = product.Id,
                    Count = 1
                };

                await _context.BasketProducts.AddAsync(basketProduct);
            }
            else
            {
                basketProduct.Count++;
                _context.BasketProducts.Update(basketProduct);
            }

            await _context.SaveChangesAsync();

            return Ok("Mehsul ugurla elave edildi");
        }


        public async Task<IActionResult> Increase(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var basket = await _context.Baskets.FirstOrDefaultAsync(b => b.UserId == user.Id);
            if (basket == null) return NotFound("basket tapilmadi");

            var basketProduct = await _context.BasketProducts.FirstOrDefaultAsync(bp => bp.Id == id && bp.BasketId == basket.Id);
            if (basketProduct == null)
            {
                return NotFound("Sebetdeki mehsul tapilmadi");
            }

            var product = await _context.Products.FindAsync(basketProduct.ProductId);
            if (product is null)
            {
                return NotFound("Mehsul tapilmadi");
            }

            if (product.Quantity == basketProduct.Count)
                return NotFound("MAX");

            basketProduct.Count++;

            _context.BasketProducts.Update(basketProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Decrease(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var basket = await _context.Baskets.FirstOrDefaultAsync(b => b.UserId == user.Id);
            if (basket == null) return NotFound("basket tapilmadi");

            var basketProduct = await _context.BasketProducts.FirstOrDefaultAsync(bp => bp.Id == id && bp.BasketId == basket.Id);
            if (basketProduct == null)
            {
                return NotFound("mehsul tapilmadi");
            }

            if (basketProduct.Count == 0)
                return NotFound("MIN");

            basketProduct.Count--;

            _context.BasketProducts.Update(basketProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var basket = await _context.Baskets.FirstOrDefaultAsync(b => b.UserId == user.Id);
            if (basket == null) return NotFound("basket tapilmadi");

            var basketProduct = await _context.BasketProducts.FirstOrDefaultAsync(bp => bp.Id == id && bp.BasketId == basket.Id);
            if (basketProduct == null)
            {
                return NotFound("sebetdeki mehsul tapilmadi");
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound("mehsul tapilmadi");

            _context.BasketProducts.Remove(basketProduct);  
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



























        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    List<BasketModel> basket;

        //    var basketCookie = Request.Cookies["basket"];
        //    if (basketCookie == null)
        //    {
        //        basket = new List<BasketModel>();
        //    }
        //    else
        //    {
        //        basket = JsonConvert.DeserializeObject<List<BasketModel>>(basketCookie);
        //    }

        //    foreach (var basketProduct in basket)
        //    {
        //        var product = await _context.Products.FindAsync(basketProduct.Id);
        //        if (product is not null)
        //        {
        //            basketProduct.Title = product.Title;
        //            basketProduct.Price = product.Price;
        //            basketProduct.StockQuantity = product.Quantity;
        //            basketProduct.PhotoName = product.PhotoName;
        //        }
        //    }

        //    return View(basket);
        //}

        //[HttpGet]
        //public async Task<IActionResult> AddAsync(int id)
        //{
        //    List<BasketModel> basket;

        //    var basketCookie = Request.Cookies["basket"];
        //    if (basketCookie is null)
        //        basket = new List<BasketModel>();
        //    else
        //        basket = JsonConvert.DeserializeObject<List<BasketModel>>(basketCookie);

        //    var product = await _context.Products.FindAsync(id);
        //    if (product is null)
        //        return NotFound("Mehsul tapilmadi");

        //    var basketProduct = basket.Find(bp => bp.Id == product.Id);
        //    if (basketProduct is not null)
        //    {
        //        if (product.Quantity == basketProduct.Count)
        //            return NotFound("Max");

        //        basketProduct.Count++;
        //    }
        //    else
        //    {
        //        basket.Add(new BasketModel
        //        {
        //            Id = product.Id,
        //            Count = 1
        //        });
        //    }


        //    Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
        //    return Ok();
        //}

        //[HttpGet]
        //public async Task<IActionResult> Increase(int id)
        //{
        //    List<BasketModel> basket;

        //    var basketCookie = Request.Cookies["basket"];
        //    if (basketCookie is null)
        //        basket = new List<BasketModel>();
        //    else
        //        basket = JsonConvert.DeserializeObject<List<BasketModel>>(basketCookie);

        //    var product = await _context.Products.FindAsync(id);
        //    if (product is null)
        //        return NotFound("Mehsul tapilmadi");

        //    var basketProduct = basket.Find(bp => bp.Id == product.Id);
        //    if (basketProduct is not null)
        //    {
        //        if (product.Quantity == basketProduct.Count)
        //            return NotFound("Max");

        //        basketProduct.Count++;
        //    }

        //    Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
        //    return RedirectToAction(nameof(Index));
        //}

        //[HttpGet]
        //public async Task<IActionResult> Decrease(int id)
        //{
        //    List<BasketModel> basket;

        //    var basketCookie = Request.Cookies["basket"];
        //    if (basketCookie is null)
        //        basket = new List<BasketModel>();
        //    else
        //        basket = JsonConvert.DeserializeObject<List<BasketModel>>(basketCookie);

        //    var product = await _context.Products.FindAsync(id);
        //    if (product is null)
        //        return NotFound("Mehsul tapilmadi");

        //    var basketProduct = basket.Find(bp => bp.Id == product.Id);
        //    if (basketProduct is not null)
        //    {
        //        if (basketProduct.Count == 0)
        //            basket.Remove(basketProduct);
        //        else
        //            basketProduct.Count--;
        //    }

        //    Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
