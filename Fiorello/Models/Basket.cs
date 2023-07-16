using Fiorello.Entities;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace Fiorello.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public Entities.User User { get; set; }
        public string UserId { get; set; }
        public ICollection<BasketProduct> BasketProducts { get; set; }
    }
}
