using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Fiorello.Areas.Admin.ViewModels.Product
{
    public class ProductIndexVM
    {
        public ProductIndexVM()
        {
            Products = new List<Models.Product>();
            CategoriesIds = new List<int>();
        }
        public List<Models.Product> Products { get; set; }

        #region FilterProperties
        public string? Title { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set;}
        public List<SelectListItem> Categories { get; set; }
        [Display(Name = "Category")]
        public List<int> CategoriesIds { get; set;}
        [Display(Name = "From")]
        public DateTime? CreatedAtStart { get; set; }
        [Display(Name = "To")]
        public DateTime? CreatedAtEnd { get; set; }



        #endregion
    }
}
