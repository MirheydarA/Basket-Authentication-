using Fiorello.Enum;
using System.ComponentModel.DataAnnotations;

namespace Fiorello.Areas.Admin.ViewModels.Product
{
    public class ProductDetailsVM
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AdditionalInfo { get; set; }
        public int Price { get; set; }
        [EnumDataType(typeof(ProductStatus))]
        public ProductStatus ProductStatusType { get; set; }
        public string PhotoName { get; set; }
        public List<IFormFile> Photos { get; set; }
        public Models.ProductCategory ProductCategory { get; set; }
    }
}
