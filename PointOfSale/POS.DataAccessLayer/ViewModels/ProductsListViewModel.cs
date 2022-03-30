using POS.DataAccessLayer.Models.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace POS.DataAccessLayer.ViewModels
{
    public class ProductsListViewModel
    {
        public ProductModel Product { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string CompanyName { get; set; }
        public int Total { get; set; }
    }
}
