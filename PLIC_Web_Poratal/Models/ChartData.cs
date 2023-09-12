using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareConnect.Models
{
    public class ChartData
    {
        public int TotalTicket { get; set; }
        public string CategoryDescription { get; set; }
        public string IssueTypeDescription { get; set; }
        public string Percentage { get; set; }
        public string region_name { get; set; }
        public string TicketType { get; set; }
        public string Products { get; set; }
    }
}
