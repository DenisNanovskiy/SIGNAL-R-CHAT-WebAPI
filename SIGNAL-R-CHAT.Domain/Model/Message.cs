using System;

namespace SIGNAL_R_CHAT.Domain
{
    public class Message
    {
        public Guid MessageId { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public Addressee Addressee { get; set; }
    }
}

