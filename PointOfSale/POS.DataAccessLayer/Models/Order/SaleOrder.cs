using POS.DataAccessLayer.Models.Company;
using POS.DataAccessLayer.Models.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace POS.DataAccessLayer.Models
{
    public class SaleOrder
    {
        [Key]
        public long Id { get; set; }
        public long InvNumber { get; set; }
        public int CompanyId { get; set; }
        public int? CustomerId { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public int PaymentTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<SaleOrderDetails> SaleOrderDetails { get; set; }
        public virtual CompanyModel Company { get; set; }
        public virtual CustomerModel Customer { get; set; }
    }


    public class SaleOrderDetails
    {
        [Key]
        public long OrderDetailId { get; set; }
        public long SaleOrderId { get; set; }
        public int ProductId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public virtual SaleOrder SaleOrder { get; set; }
        public virtual Product Product { get; set; }
    }
}
