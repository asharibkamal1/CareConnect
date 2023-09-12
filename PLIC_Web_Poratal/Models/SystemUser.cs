using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class SystemUser
    {

        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [RegularExpression("^[0-9]{5}-[0-9]{7}-[0-9]$", ErrorMessage = "CNIC No must follow the XXXXX-XXXXXXX-X format!")]
        public string CNIC     { get; set; }

        [Required]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "Mobile No must follow the XXXXXXXXXXX format!")]
        public string PhoneNo { get; set; }

        [Required]
        public string Designation { get; set; }

        public int SuperAdmin { get; set; }
        public int circleId { get; set; }
        public int RegionId { get; set; }
        public int FieldUnitId { get; set; }
        public string GroupId { get; set; }

        [Required]
        public int Level { get; set; }
        //public List<Circle> circles { get; set; }

        public string CircleName { get; set; }
        public string RegionName { get; set; }
        public string FielUnitName { get; set; }
        public string AdminPassword { get; set; }
        
        public List<Group> groups { get; set; }

    }
}
