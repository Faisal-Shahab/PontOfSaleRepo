using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Model
{
    public class SalePurchaseOrderViewModel
    {
        public long Id { get; set; }
        public long InvNumber { get; set; }
        public int CompanyId { get; set; }
        public int? CustomerId { get; set; }
        public int SupplierId { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public int PaymentTypeId { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}
