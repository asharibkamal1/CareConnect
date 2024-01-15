using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareConnect.Models
{
    public class TarrifCalculatorModel
    {
        public string Source {  get; set; }
        public string SourceCCP { get; set; } 
        public string Destination { get; set; } 
        public string DestinationCCP { get; set; }
        public string TariffType { get; set; }
        public string CategoryCCP { get; set; }
        public int TxtQuantity { get; set; }
        public int TxtUnit { get; set; }
        public int TxtWeight { get; set; }
        public int TxtTotalweight { get; set; }
        public int LblWeight { get; set; }
        public int LblQty { get; set; }
        public int TxtAmount { get; set; }
        public int TxtTaxAmount { get; set; }
        public int LblAmount { get; set; }

    }
}
