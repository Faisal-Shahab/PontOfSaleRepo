using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Model
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ArabicName { get; set; }

        [StringLength(15)]
        public string Size { get; set; }
        [StringLength(60)]
        public string Color { get; set; }
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal DiscoutPercentage { get; set; }
        [StringLength(100)]
        public string Barcode { get; set; }
        public bool IsTaxApplied { get; set; }        
    }
}
