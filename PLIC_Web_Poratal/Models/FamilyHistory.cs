using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class FamilyHistory
    {
        public string relation { get; set; }
        public int age { get; set; }
        public string healthState { get; set; }
        public string alive { get; set; }
        public int deathAge { get; set; }
        public int deathYear { get; set; }
        public string deathCause { get; set; }
    }
}
