using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class UserPerformanceSummary
    {
        public string userName { get; set; }
        public int morethan60days { get; set; }
        public int morethan30days { get; set; }
        public int morethan15days { get; set; }
        public int recievedin15days { get; set; }
        public int totalpending { get; set; }
        public int resolvedin30days { get; set; }
    }
}
