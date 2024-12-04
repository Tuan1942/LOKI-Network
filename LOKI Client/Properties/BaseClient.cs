using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Client.Properties
{
    public class BaseClient
    {
        //static string Ip = "192.168.1.244";
        //static string Ip = "0.0.0.0";
        static string Ip = "localhost";
        static string Port = "3000";
        public static readonly string ServerAddress = $"https://{Ip}:{Port}/";
        public static readonly string WebSocketAddress = $"wss://{Ip}:{Port}/ws";
        public static readonly string ChatHubAddress = $"https://{Ip}:{Port}/chatHub";
    }
}
