using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareConnect.Models
{
    public class UpdateTicketModel
    {

        public string remarks { get; set; }
        public string ticketdid { get; set; }
        public string activity { get; set; }
        public string userid { get; set; }
        public string city { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string region { get; set; }
        public int TicketTypeDropdownid { get; set; }
        public int issuetypeid { get; set; }
        public string complainercell { get; set; }
    }
}
