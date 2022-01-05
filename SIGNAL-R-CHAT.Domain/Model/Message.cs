using System;

namespace SIGNAL_R_CHAT.Domain
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public Person FromPerson{ get; set; }
        public WorkGroup ToGroup { get; set; }
        public Guid ToGroupId { get; set; }
    }
}
 
