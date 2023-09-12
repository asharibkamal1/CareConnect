using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class CompliantViewModel
    {
        public List<PostComplaint> postComplaints { get; set; }
        public List<PreComplaint> preComplaints { get; set; }
        public List<Group> groups { get; set; }
    }
}
