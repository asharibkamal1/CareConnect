using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CareConnect.Models
{
    public class TrackingGenerateViewModel
    {
        public DataSet BookingDetail { get; set; }
        public DataSet TicketType { get; set; }
        public DataSet CRMReportDetail { get; set; }
        public DataSet TicketCatType { get; set; }
        public DataSet AccountOpening { get; set; }

        public DataSet CODLedgerReport { get; set; }
        public DataSet CODInvoiceReport { get; set; }
        public DataSet AllTerminals { get; set; }
        public DataSet TicketIssueTypeDescription { get; set; }
        public DataSet Sub_Terminal { get; set; }
        public DataSet InfoSubcategory { get; set; }
        public DataSet PriorityDS { get; set; }
        public DataSet CityDS { get; set; }

        public DataSet RegionDS { get; set; }
        public DataSet Category { get; set; }

        public DataSet TicketDetails { get; set; }
        public DataSet TrackingHistory { get; set; }
        public DataSet SearchTicketDetail { get; set; }

        public DataSet SearchClaimDetail { get; set; }
        public DataSet TerminalAddressReportDS { get; set; }
        public DataSet ClaimCategoryDS { get; set; }
        public DataSet TrackingDetailsforClaimreport {  get; set; }
        public DataSet ClaimIssueTypeDescription { get; set; }
        public DataSet ClaimDetails { get; set; }

        public DataSet TariffDetails { get; set; }
        public DataSet TariffDetailsTerminals { get; set; }
        public DataSet TariffDetailsCustomer { get; set; }






    }
}
