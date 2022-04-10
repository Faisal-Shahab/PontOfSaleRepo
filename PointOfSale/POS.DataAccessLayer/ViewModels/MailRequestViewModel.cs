using System;
using System.Collections.Generic;
using System.Text;

namespace POS.DataAccessLayer.ViewModels
{
    public class MailRequestViewModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
