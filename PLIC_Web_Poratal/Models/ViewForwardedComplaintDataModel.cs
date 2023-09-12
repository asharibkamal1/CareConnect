using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class ViewForwardedComplaintDataModel
    {
        public int complaintId { get; set; }
        public PostComplaint postComplaint { get; set; }

        public List<ForwardComplaintHistory> forwardComplaintHistories { get; set; }

        public int TrackingID { get; set; }

    }
}
