using Fiorello.DAL;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fiorello.ViewModels.Product
{
    public class ProductndexVM 
    {
        public ProductndexVM()
        {
            List<Models.Product> products = new List<Models.Product>();
        }
        public Models.Product Products { get; set; }
    }
}
