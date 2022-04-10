using System;
using System.Collections.Generic;
using System.Text;

namespace POS.DataAccessLayer.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public DateTime DateCreated { get; set; }
        public int? CompanyId { get; set; }
        public bool IsActive { get; set; }
    }
}
