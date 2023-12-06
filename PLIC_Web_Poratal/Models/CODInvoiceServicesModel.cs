using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CareConnect.Models
{


    public class CODInvoiceServicesModel
    {

        public int customer_id { get; set; }
        public DateTime datefrom { get; set; }
        public DateTime dateto { get; set; }
        public DateTime invoice_date { get; set; }

    }
}
