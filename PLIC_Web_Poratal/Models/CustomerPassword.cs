using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class CustomerPassword
    {
        [Required]
        [StringLength(20,MinimumLength =8, ErrorMessage = "Password must be atleast 8 characters in length.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d$@$!%*?&]{6,}$", ErrorMessage = "Password must contain: 8 characters atleast 1 UpperCase Alphabet, 1 LowerCase Alphabet and 1 Number.")]
        public string Password { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be atleast 8 characters in length.")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
    }
}
