using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace POS.DataAccessLayer.Models.Payment
{
    public class PaymentTypeModel
    {
        [Key]
        public int PaymentId { get; set; }
        [StringLength(160)]
        public string Name { get; set; }
    }
}
