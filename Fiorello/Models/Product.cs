using Fiorello.Enum;
using System.ComponentModel.DataAnnotations;

namespace Fiorello.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AdditionalInfo { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string PhotoName { get; set; }
        public ICollection<ProductPhoto> Photos { get; set; }

        [EnumDataType(typeof(ProductStatus))]
        public ProductStatus ProductStatusType { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }

    }
}
