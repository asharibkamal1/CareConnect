using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class ViewModel
    {
        public int option { get; set; }
        public string userName { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;
        public string dateTime { get; set; } 
        public InsurantInfo info { get; set; }
        public Occupation  occupation { get; set; }
        public List<FamilyHistory> familyHistory { get; set; }
        public List<Nominee> nominees { get; set; }

        public PostLoginComplaint postLoginComplaint { get; set; }

        public PostLoginComplaint postLoginComplaint1 { get; set; }
        public PostLoginComplaint postLoginComplaint2 { get; set; }
        public PreLoginComplaint preLoginComplaint { get; set; }
        public PreLoginComplaint preLoginComplaint1 { get; set; }
        public List<Circle> circles { get; set; }
        public List<Region> regions { get; set; }
        public Circle circle { get; set; }
        public Region region { get; set; }
        public List<SystemUser> systemUsers { get; set; }
        public List<ComplaintsSummary> complaintsSummaries { get; set; }
        public List<UserPerformanceSummary> userPerformanceSummaries { get; set; }

        public ViewDataModel viewDataModel { get; set; }


    }
}
