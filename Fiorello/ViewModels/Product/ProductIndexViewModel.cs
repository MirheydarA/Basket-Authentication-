using Fiorello.DAL;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fiorello.ViewModels.Product
{
    public class ProductIndexViewModel 
    {
        public ProductIndexViewModel()
        {
            List<Models.Product> products = new List<Models.Product>();
        }
        public List<Models.Product> Products { get; set; }
    }
}
