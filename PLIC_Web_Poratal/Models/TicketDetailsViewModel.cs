using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CareConnect.Models
{
    public class TicketDetailsViewModel
    {
        public DataSet TicketDetails { get; set; }
        public DataSet TicketHistory { get; set; }
        public DataSet BookingDetail { get; set; }
        public DataSet TicketStatus { get; set; }
        public DataSet TicketType { get; set; }
        public string userid { get; set; }
        public string roleid { get; set; }
        //public Models CurrentUser { get; set; }
    }
}
