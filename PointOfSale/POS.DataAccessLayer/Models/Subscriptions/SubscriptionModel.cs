using POS.DataAccessLayer.Models.Company;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace POS.DataAccessLayer.Models.Subscriptions
{
    public class SubscriptionModel
    {
        [Key]
        public int SubscriptionId { get; set; }
        [StringLength(160)]
        [Required]
        public string Name { get; set; }
        [StringLength(500)]
        public string ArabicName { get; set; }
        public decimal Price { get; set; }
        public int MaxAccounts { get; set; }
        public int MaxOrders { get; set; }
        [StringLength(100)]
        public string CreateBy { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<CompanySubscriptionModel> CompanySubscriptions { get; set; }
    }

    public class CompanySubscriptionModel
    {
        [Key]
        public int CompanySubscriptionId { get; set; }
        public int SubscriptionId { get; set; }
        public int CompanyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [StringLength(100)]
        public string CreateBy { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual SubscriptionModel Subscription { get; set; }
        public virtual CompanyModel Company { get; set; }
    }
}
