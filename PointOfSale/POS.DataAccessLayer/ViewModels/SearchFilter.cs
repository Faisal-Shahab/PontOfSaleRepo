using System;
using System.Collections.Generic;
using System.Text;
using static POS.DataAccessLayer.ViewModels.Enums;

namespace POS.DataAccessLayer.ViewModels
{
    public class SearchFilter
    {
        public int PageLength { get; set; } = 10;
        public int Start { get; set; }
        public OrderDirection OrderDirection { get; set; }
        public string OrderBy { get; set; }
        public string SearchTerm { get; set; }
        public int Draw { get; set; }
        public int PageNum { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public bool ExportByOrderDate { get; set; } = false;
        public int LanguageId { get; set; } = 1;        
    }

    public class Enums
    {
        public enum OrderDirection
        {
            Descending = 1,
            Ascending = 2
        }
    }
}
