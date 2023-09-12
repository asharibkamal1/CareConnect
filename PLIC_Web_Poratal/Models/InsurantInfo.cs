using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class InsurantInfo
    {
        public string policyTerm  { get; set; }
        public string  policyType  { get; set; }
        public decimal sumAssured  { get; set; }
        public DateTime riskDate { get; set; }
        public string gender { get; set; }
        public string maritalStatus { get; set; }
        public DateTime birthDate { get; set; }
        public string birthPlace { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string phone { get; set; }
        public string religion { get; set; }

        //public string policyTerm { get; set; }
        //public string policyType { get; set; }
        //public decimal sumAssured { get; set; }
        //public DateTime riskDate { get; set; }
        //public string gender { get; set; }
        //public string maritalStatus { get; set; }
        //public DateTime birthDate { get; set; }
        //public string birthPlace { get; set; }
        //public string address { get; set; }
        //public string city { get; set; }
        //public string phone { get; set; }
        //public string religion { get; set; }

    }
}
