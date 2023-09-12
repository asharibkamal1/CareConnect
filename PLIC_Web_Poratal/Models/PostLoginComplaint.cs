using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class PostLoginComplaint
    {
        [Required]
        public string policyNo { get; set; }

        public string cnicNo { get; set; }
        public string fieldUnit { get; set; }
        [Required]
        public string fieldUnitId { get; set; }
        [Required]
        public string complaintType { get; set; }
        [Required]
        public string complaintOrigin { get; set; }

        [Required]
        public string comments { get; set; }

        public int adminId { get; set; }
        
        [Required]
        public string mobileNumber { get; set; }

        public List<FieldUnits> fieldUnits { get; set; }

        public List<IFormFile> documents { get; set; }
        public int totalPostcomplaints { get; set; }
        public int totalResolvedPostcomplaints { get; set; }
        public int totalUnResolvedPostcomplaints { get; set; }
        public int totalResolvedComplaints { get; set; }

        public string newMobileNumber { get; set; }

    }
}
