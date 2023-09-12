using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class ForwardComplaintHistory
    {
        public DateTime forwardedDate { get; set; }
        public string forwardBy { get; set; }
        public string forwardTo { get; set; }
        public string forwardComments { get; set; }
        public bool Isforward { get; set; }
        public string userName { get; set; }

    }
}
