using POS.DataAccessLayer.Models.Company;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace POS.DataAccessLayer.Models.Category
{
    public class ProductModel
    {
        [Key]
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
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
        [StringLength(100)]
        public string CreateBy { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual CategoryModel Category { get; set; }
        public virtual CompanyModel Company { get; set; }
        public virtual ICollection<ProductDescriptionModel> ProductDescriptions { get; set; }
    }

    public class ProductDescriptionModel
    {
        [Key]
        public int ProductDescriptionId { get; set; }
        public int ProductId { get; set; }
        [StringLength(160)]
        public string Name { get; set; }
        public int LanguageId { get; set; }
        [StringLength(100)]
        public string CreateBy { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ProductModel Product { get; set; }
    }
}
