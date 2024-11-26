using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Model.Models
{
    public class SendMessageRequest
    {
        public string Message { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}
