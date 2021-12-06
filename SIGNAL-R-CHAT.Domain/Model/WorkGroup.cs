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

        public string Description { get; set; }

        public DateTime СreationTime { get; set; }

        public List<Message> Messages { get; set; }

        public List<Person> Persons { get; set; }
    }
}
        