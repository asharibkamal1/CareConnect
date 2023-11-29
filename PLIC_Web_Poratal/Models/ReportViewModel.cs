using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CareConnect.Models
{
    public class ReportViewModel
    {

        public DataSet Report_rpt_ticketdetails { get; set; }
        public DataSet Report_ticket_track_details { get; set; }
        public DataSet Report_rpt_Ticket_SMS { get; set; }
        public DataSet Report_rpt_Ticket_Close { get; set; }
        public DataSet Report_rpt_Customer_Ledger {  get; set; }
        
    }
}
