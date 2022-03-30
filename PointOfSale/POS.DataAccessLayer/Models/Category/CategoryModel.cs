using POS.DataAccessLayer.Models.Company;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace POS.DataAccessLayer.Models.Category
{
    public class CategoryModel
    {
        [Key]
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        [StringLength(100)]
        public string CreateBy { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual CompanyModel Company { get; set; }
        public virtual ICollection<CategoryDescriptionModel> CategoryDescriptions { get; set; }

    }

    public class CategoryDescriptionModel
    {
        [Key]
        public int CategoryDescriptionId { get; set; }
        public int CategoryId { get; set; }
        public int LanguageId { get; set; }
        [StringLength(160)]
        public string Name { get; set; }
        [StringLength(100)]
        public string CreateBy { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual CategoryModel Category { get; set; }
    }
}
