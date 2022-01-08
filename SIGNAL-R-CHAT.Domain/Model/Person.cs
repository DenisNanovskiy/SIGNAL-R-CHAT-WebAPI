using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace SIGNAL_R_CHAT.Domain
{
   public class Person : IdentityUser
    {

        public string Name { get; set; }

        public ICollection<WorkGroup> WorkGroups  { get; set; }
    }
}
