using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class CustomerPostComplaints
    {
        public int complaintId { get; set; }
        public DateTime complaintDate  { get; set; }
        public DateTime resolvedDate { get; set; }
        public string adminComments { get; set; }
        public string comments { get; set; }
        public Boolean isResolved { get; set; }
        public Boolean isClosed { get; set; }
        public double complaint_Id { get; set; }
        public string reopenComments { get; set; }

        public Boolean isReopend { get; set; }
    }
}
