using System.ComponentModel.DataAnnotations;

namespace Fiorello.Areas.Admin.ViewModels.ProductCategory
{
    public class ProductCategoryCreateVM
    {
        [Required , MinLength(3, ErrorMessage ="Adin uzunlugu minimum 3 simvol olmalidir")]
        public string Name  { get; set; }
    }
}
