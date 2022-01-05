using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGNAL_R_CHAT.Domain
{
    public class WorkGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Person Admin { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
        