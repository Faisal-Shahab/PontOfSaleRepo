using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Model
{
    public class EmailViewModel
    {
        public string Subject { get; set; }
        public string To { get; set; }
        public string MessageBody { get; set; }
    }
}
