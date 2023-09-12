using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class CustomerSignUp
    {
        [Required]
        public string CNIC { get; set; }
       
        [Required]
        public string MobileNo { get; set; }
    }
}
