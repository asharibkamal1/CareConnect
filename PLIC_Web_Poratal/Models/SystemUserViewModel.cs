using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLIC_Web_Poratal.Models
{
    public class SystemUserViewModel
    {
        public List<SystemUser> systemUsers { get; set; }
        public List<Group> groups { get; set; }
    }
}
