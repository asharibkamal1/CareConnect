using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareConnect.Models
{
    public class CreateAccountOpeningModel
    {

        public int accounttype { get; set; }
        public string accounttypename { get; set; }
        public int city { get; set; }
        public string cityname { get; set; }
        public int location { get; set; }
        public string locationname { get; set; }
        public int region { get; set; }
        public string regionname { get; set; }
        public string customername { get; set; }
        public string contact { get; set; }
        public string avgship { get; set; }
        public string avgwgt { get; set; }
        public string businessnature { get; set; }
        public string emailaddress { get; set; }
        public string frnbusinesslocation { get; set; }
        public int frnownproperty { get; set; }
        
        public string Remarks { get; set; }
       
      
        

    }
}
