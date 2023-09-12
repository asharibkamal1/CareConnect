using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class Policy1
    {
        public string policy { get; set; }
        public int iid { get; set; }
        public string cnic { get; set; }
        public string name { get; set; }
        public DateTime periodTo { get; set; }
        public DateTime nextDue { get; set; }
        public DateTime maturityDate { get; set; }
        public string monthlyPremium { get; set; }
        public string premiumMode { get; set; }
        public string policyStatus { get; set; }
    }
}
