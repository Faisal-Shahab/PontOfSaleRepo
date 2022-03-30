using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace POS.DataAccessLayer.Models
{
    public class SaleOrder
    {
        [Key]
        public long OrderId { get; set; }
        public long InvNumber { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public decimal Total { get; set; }
        public int PaymentTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }


    public class SaleOrderDetails
    {
        [Key]
        public long OrderDetailId { get; set; }
        public long OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
