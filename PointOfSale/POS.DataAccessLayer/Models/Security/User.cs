using Microsoft.AspNetCore.Identity;
using POS.DataAccessLayer.Models.Company;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace POS.DataAccessLayer.Models.Security
{
    public class User : IdentityUser
    {
        [StringLength(160)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int? CompanyId { get; set; }
        [StringLength(100)]
        public string CreatedBy { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow.AddHours(3);
        public DateTime DateUpdated { get; set; }
        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }
        public virtual CompanyModel Company { get; set; }
    }
}
