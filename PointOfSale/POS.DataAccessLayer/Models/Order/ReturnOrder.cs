using POS.DataAccessLayer.Models.Company;
using POS.DataAccessLayer.Models.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.DataAccessLayer.Models.Order
{
    public class ReturnOrder
    {
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public int? CustomerId { get; set; }
        public long SaleOrderId { get; set; }
        public long InvNumber { get; set; }
        public decimal Total { get; set; }
        public int PaymentTypeId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [StringLength(100)]
        public string CreatedBy { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public virtual CompanyModel Company { get; set; }
        public virtual CustomerModel Customer { get; set; }
        public virtual SaleOrder SaleOrder { get; set; }
        public virtual ICollection<ReturnOrderDetail> ReturnOrderDetails { get; set; }
    }

    public class ReturnOrderDetail
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("ReturnOrder")]
        public long ReturnOrderId { get; set; }
        public int ProductId { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [StringLength(100)]
        public string CreatedBy { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public virtual ReturnOrder ReturnOrder { get; set; }
        public virtual Product Product { get; set; }
    }
}
