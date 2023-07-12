using Fiorello.DAL;
using Fiorello.ViewModels.Basket;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fiorello.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var basket = Request.Cookies["basket"];
            return Ok(basket);
        }


        public async Task<IActionResult> AddAsync(int id)
        {
            List<BasketModel> basket;

            var basketCookie = Request.Cookies["basket"];
            if (basketCookie is null)
                basket = new List<BasketModel>();
            else
                basket = JsonConvert.DeserializeObject<List<BasketModel>>(basketCookie);

            var product = await _context.Products.FindAsync(id);
            if (product is null)
                return NotFound("Mehsul tapilmadi");

            var basketProduct = basket.Find(bp => bp.Id == product.Id);
            if (basketProduct is not null)
            {
                basketProduct.Count++;
            }
            else
            {
                basket.Add(new BasketModel
                {
                    Id = product.Id,
                    Count = 1
                }); 
            }


            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
            return Ok();
        }
    }
}
