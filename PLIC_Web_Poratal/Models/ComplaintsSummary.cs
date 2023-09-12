using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class ComplaintsSummary
    {
        public string complaintOrigin { get; set; }
        public int morethan60days { get; set; }
        public int morethan30days { get; set; }
        public int morethan15days { get; set; }
        public int atleast15days { get; set; }
    }
}
