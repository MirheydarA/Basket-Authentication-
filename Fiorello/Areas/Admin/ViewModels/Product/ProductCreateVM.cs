﻿using Fiorello.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Fiorello.Areas.Admin.ViewModels.Product
{
    public class ProductCreateVM
    {
        public ProductCreateVM()
        {
            Photos = new List<IFormFile>();
        }

        [Required, MinLength(3, ErrorMessage = "Adin uzunlugu minimum 3 simvol olmalidir")]
        public string Name{ get; set; }

        [Required, MinLength(3, ErrorMessage = "Adin uzunlugu minimum 3 simvol olmalidir")]
        public string Title { get; set; }
        public string Description { get; set; }

        [Display(Name = "Additional Info")]
        public string AdditionalInfo { get; set; }

        public int Quantity { get; set; }

        [Required] 
        public int Price { get; set; }
        public IFormFile Photo { get; set; }
        public List<IFormFile> Photos { get; set; }

        [EnumDataType(typeof(ProductStatus))]
        public ProductStatus ProductStatusType { get; set; }

        [Required]
        [Display(Name = "Product category id")]
        public int ProductCategoryId { get; set; }
        public List<SelectListItem>? ProductCategories { get; set; }
    }
}
