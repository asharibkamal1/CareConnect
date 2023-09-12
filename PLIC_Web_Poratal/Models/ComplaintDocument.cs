using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class ComplaintDocument
    {
        public double ComplaintId { get; set; }
        public DateTime entryDate { get; set; }
        public string documentName { get; set; }
    }
}
