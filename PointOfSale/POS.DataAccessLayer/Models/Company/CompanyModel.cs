using POS.DataAccessLayer.Models.Customer;
using POS.DataAccessLayer.Models.Order;
using POS.DataAccessLayer.Models.Security;
using POS.DataAccessLayer.Models.Subscriptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace POS.DataAccessLayer.Models.Company
{
    public class CompanyModel
    {
        [Key]
        public int CompanyId { get; set; }
        [StringLength(160)]
        [Required]
        public string Name { get; set; }
        [StringLength(500)]
        public string ArabicName { get; set; }
        [StringLength(1000)]
        public string Address { get; set; }
        public string FaxNo { get; set; }
        public string ContactNo { get; set; }
        [StringLength(60)]
        public string Email { get; set; }
        [StringLength(60)]
        public string CrNumber { get; set; }
        [StringLength(60)]
        public string TaxNumber { get; set; }
        [StringLength(500)]
        public string ThankyouNote { get; set; }
        [StringLength(500)]
        public string Logo { get; set; }
        [StringLength(50)]
        public string Printer { get; set; }
        [StringLength(100)]
        public string CreateBy { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<CustomerModel> Customers { get; set; }
        public virtual ICollection<SupplierModel> Suppliers { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<CompanySubscriptionModel> CompanySubscriptions { get; set; }
        public virtual ICollection<SaleOrder> SaleOrders { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
