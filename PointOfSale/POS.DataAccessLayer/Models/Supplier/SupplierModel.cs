using POS.DataAccessLayer.Models.Company;
using POS.DataAccessLayer.Models.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace POS.DataAccessLayer.Models
{
    public class SupplierModel
    {
        [Key]
        public int SupplierId { get; set; }
        [StringLength(160)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Address { get; set; }
        public string ContactNo { get; set; }
        [StringLength(60)]
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public int CompanyId { get; set; }
        [StringLength(100)]
        public string CreateBy { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual CompanyModel Company { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
