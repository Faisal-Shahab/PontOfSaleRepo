using POS.DataAccessLayer.Models;
using POS.DataAccessLayer.Models.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Model
{
    public class SaleOrderViewModel
    {
        public SaleOrder saleOrder { get; set; }
        public List<SaleOrderDetails> details { get; set; }
        public CompanyModel CompanyDetails { get; set; }
    }
}
