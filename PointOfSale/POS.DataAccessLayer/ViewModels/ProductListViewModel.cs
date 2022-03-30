using System;
using System.Collections.Generic;
using System.Text;

namespace POS.DataAccessLayer.ViewModels
{
    public class ProductListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Discount { get; set; }
        public decimal Price { get; set; }
    }
}
