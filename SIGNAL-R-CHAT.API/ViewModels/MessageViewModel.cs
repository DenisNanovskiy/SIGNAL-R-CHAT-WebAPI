using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGNAL_R_CHAT.API.ViewModels
{
    public class MessageViewModel
    {
        public string Text { get; set; }
        public string DateTime { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}