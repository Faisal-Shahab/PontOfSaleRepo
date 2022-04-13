using POS.DataAccessLayer.Models;
using POS.DataAccessLayer.Models.Company;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace POS.DataAccessLayer.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
       // public string ImageUrl { get; set; }
        public int CompanyId { get; set; }
        public string CreateBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }= DateTime.UtcNow;
        public virtual CompanyModel Company { get; set; }
    }

}
