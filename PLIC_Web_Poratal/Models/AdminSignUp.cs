using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class AdminSignUp
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string PhoneNo { get; set; }
    }
}
