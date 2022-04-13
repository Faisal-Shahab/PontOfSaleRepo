using POS.DataAccessLayer.Models;
using POS.DataAccessLayer.Models.Company;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace POS.DataAccessLayer.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        //public string ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public int CompanyId { get; set; }
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; } = 0.0M;
        public decimal SalePrice { get; set; }
        public decimal? Discount { get; set; }
        public string Barcode { get; set; }
        public bool IsTaxApplied { get; set; }
        public string CreateBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public virtual Category Category { get; set; }
        public virtual CompanyModel Company { get; set; }
    }

}
