using POS.DataAccessLayer.Models.Company;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace POS.DataAccessLayer.Models.Supplier
{
    public class SupplierTransaction
    {
        [Key]
        public long Id { get; set; }
        public long? PurchaseOrderId { get; set; }
        public int SupplierId { get; set; }
        public int CompanyId { get; set; }
        public decimal Balance { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        [StringLength(100)]
        public string CreateBy { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow.AddHours(3);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(3);
        public virtual CompanyModel Company { get; set; }
        public virtual SupplierModel Supplier { get; set; }
    }
}
