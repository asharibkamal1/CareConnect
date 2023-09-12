using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using System.ComponentModel.DataAnnotations;


namespace PLIC_Web_Poratal.Models
{
    public class Account
    {
        //[Required(ErrorMessage ="Please Enter Correct CNIC")]
        //[Required]
        [Display(Name ="Enter Login ID")]
        //[RegularExpression("^[0-9]{5}-[0-9]{7}-[0-9]$", ErrorMessage = "CNIC No must follow the XXXXX-XXXXXXX-X format!")]
        public string LoginId { get; set; }

        public string UserName { get; set; }

        public string RoleID { get; set; }
        //[Required(ErrorMessage = "Please Enter Correct Mobile Number")]
        //[Required]
        [Display(Name = "Enter Password")]
        public string Password { get; set; }
        
    }
}
