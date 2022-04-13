using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Model
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public string Name { get; set; }        
        public string ArabicName { get; set; }
    }
}
