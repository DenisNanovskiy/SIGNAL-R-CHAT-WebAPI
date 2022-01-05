using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGNAL_R_CHAT.Domain
{
   public class Person
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<WorkGroup> WorkGroups  { get; set; }
    }
}
