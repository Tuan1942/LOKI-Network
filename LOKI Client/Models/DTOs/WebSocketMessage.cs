using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Client.Models.DTOs
{
    public class WebSocketMessage
    {
        public string Type { get; set; }
        public string NotificationMessage { get; set; }
        public string Content { get; set; }
    }
}
