using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGNAL_R_CHAT.Domain
{
  public class Addressee
    {
        public Guid RecipientId { get; set; }
        public int Status { get; set; }

        public DateTime ReadingTime { get; set; }
    }
}
