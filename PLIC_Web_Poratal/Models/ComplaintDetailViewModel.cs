using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class ComplaintDetailViewModel
    {
        public PostComplaint postComplaint { get; set; }
        public List<ComplaintDocument> complaintDocuments { get; set; }
    }
}
