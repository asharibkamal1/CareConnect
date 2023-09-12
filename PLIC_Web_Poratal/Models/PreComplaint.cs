using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class PreComplaint
    {
        public int srNo { get; set; }
        public double complaintId { get; set; }
        public DateTime entryDate { get; set; }
        public string insurantName { get; set; }
        public string cnic { get; set; }
        public string policyNo { get; set; }
        public string oldPhoneNo { get; set; }
        public string newPhoneNo { get; set; }
        public string region { get; set; }
        public DateTime resolveDate { get; set; }
        public int adminId { get; set; }

     
    }
}
