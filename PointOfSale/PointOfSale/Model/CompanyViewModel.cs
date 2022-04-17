using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Model
{
    public class CompanyViewModel
    {
        public int CompanyId { get; set; }
        [StringLength(160)]
        [Required]
        public string Name { get; set; }
        [StringLength(500)]
        public string ArabicName { get; set; }
        [StringLength(1000)]
        public string Address { get; set; }
        public string FaxNo { get; set; }
        public string ContactNo { get; set; }
        [StringLength(60)]
        public string Email { get; set; }
        [StringLength(60)]
        public string CrNumber { get; set; }
        [StringLength(60)]
        public string TaxNumber { get; set; }
        public string Logo { get; set; }
        public string Printer { get; set; }
        [StringLength(500)]
        public string ThankyouNote { get; set; }
        public string UserEmail  { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string Image { get; set; }
    }
}
