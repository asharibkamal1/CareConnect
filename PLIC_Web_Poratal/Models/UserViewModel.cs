using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareConnect.Models
{
    public class UserViewModel
    {
        public string LoginId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RoleID { get; set; }
    }
}
