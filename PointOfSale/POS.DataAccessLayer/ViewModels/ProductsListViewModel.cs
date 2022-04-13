using POS.DataAccessLayer.Models;

namespace POS.DataAccessLayer.ViewModels
{
    public class ProductsListViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }                                
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; } = 0.0M;
        public decimal SalePrice { get; set; }
        public decimal? Discount { get; set; }
        public string CategoryName { get; set; }       
        public int Total { get; set; }
    }
}
