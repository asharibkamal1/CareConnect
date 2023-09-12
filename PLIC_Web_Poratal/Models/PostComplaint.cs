using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class PostComplaint
    {
        public int srNo { get; set; }
        public double complaintId { get; set; }
        public DateTime entryDate { get; set; }
        public string policyNo { get; set; }
        public string phoneNo { get; set; }
        public string region { get; set; }
        public string fieldUnit  { get; set; }
        public string complaintType { get; set; }
        public string comments  { get; set; }
        public string adminComments  { get; set; }
        public DateTime resolveDate  { get; set; }
        public int adminId { get; set; }
        public string adminName { get; set; }
        public string reopenComments { get; set; }
        public DateTime reopendDate { get; set; }

        public int ? groupId { get; set; }

        public int groupNewId { get; set; }
        public bool isForward { get; set; }
        public bool isResolved { get; set; }

        public string groupDescription { get; set; }
        public int documentCount { get; set; }

    }
}
