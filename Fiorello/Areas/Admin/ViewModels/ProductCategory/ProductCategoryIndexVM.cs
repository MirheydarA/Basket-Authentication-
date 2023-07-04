using Fiorello.Models;

namespace Fiorello.Areas.Admin.ViewModels.ProductCategory
{
    public class ProductCategoryIndexVM
    {
        public ProductCategoryIndexVM()
        {
            ProductCategories = new List<Models.ProductCategory>();
        }
        public List<Models.ProductCategory> ProductCategories { get; set; }
    }
}
