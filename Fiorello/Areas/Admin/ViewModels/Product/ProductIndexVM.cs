namespace Fiorello.Areas.Admin.ViewModels.Product
{
    public class ProductIndexVM
    {
        public ProductIndexVM()
        {
            Products = new List<Models.Product>();
        }
        public List<Models.Product> Products { get; set; }
    }
}
