using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LOKI_Client.Models.Objects
{
    public class FileMetadata
    {
        public IFormFile File { get; set; }
        public BitmapImage Preview { get; set; } // Preview image for display
    }
}
