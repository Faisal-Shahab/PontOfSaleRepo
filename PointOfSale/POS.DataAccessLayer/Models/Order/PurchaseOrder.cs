using POS.DataAccessLayer.Models.Company;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace POS.DataAccessLayer.Models.Order
{
    public class PurchaseOrder
    {
        [Key]
        public long OrderId { get; set; }
        public long InvNumber { get; set; }
        public int CompanyId { get; set; }
        public int SupplierId { get; set; }
        public decimal Total { get; set; }
        public int PaymentTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }
        public virtual CompanyModel Company { get; set; }
        public virtual SupplierModel Supplier { get; set; }
    }

    public class PurchaseOrderDetails
    {
        [Key]
        public long OrderDetailId { get; set; }
        [ForeignKey("PurchaseOrder")]
        public long OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual Product Product { get; set; }
    }
}
