﻿using System;
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
        public DataSet TicketCatType { get; set; }
        public DataSet TicketIssueTypeDescription { get; set; }
        public DataSet PriorityDS { get; set; }
        public DataSet CityDS { get; set; }

        public DataSet RegionDS { get; set; }

        public DataSet TicketDetails { get; set; }
        public DataSet TrackingHistory { get; set; }
        public DataSet SearchTicketDetail { get; set; }

    }
}