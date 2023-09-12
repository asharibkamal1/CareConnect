using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class PreLoginComplaint
    {
        [Required]
        public string cnic { get; set; }
        [Required]
        [RegularExpression(@"^\(?([0]{1})\)?[-. ]?([3]{1})[-. ]?([0-9]{9})$",
                   ErrorMessage = "Please enter mobile number in valid formate e.g(03000000000)")]
        public string mobileNumber { get; set; }

        //[Required]
        //public string policyNo { get; set; }

        [Required]
        public string region { get; set; }

        //public List<Policy> policies { get; set; }
        public List<Region> regions { get; set; }

        public int totalPrecomplaints { get; set; }
        public int totalRsolvedPrecomplaints { get; set; }
    }
}
