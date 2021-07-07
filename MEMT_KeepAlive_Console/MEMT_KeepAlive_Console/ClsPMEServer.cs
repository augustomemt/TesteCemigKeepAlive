using MEMT_KeepAlive.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MEMT_KeepAlive
{
    internal class ClsPMEServer
    {
        BaseContext _context = new BaseContext();
        public ClsPMEServer()
        {
            ServerName = Dns.GetHostEntry(Dns.GetHostName()).HostName;
        }
        public ClsPMEServer(string serverIP)
        {
            ServerIP = serverIP;
            ServerName = Dns.GetHostEntry(IPAddress.Parse(ServerIP)).HostName;
        }
        public ClsPMEServer(string serverIP, bool primary)
        {
            ServerIP = serverIP;
            ServerName = Dns.GetHostEntry(IPAddress.Parse(ServerIP)).HostName;
            Primary = primary;
            var serverPrimary = _context.KeepAlive.Where(s => s.ServerIP == ServerIP).FirstOrDefault();
            if ( serverPrimary != null )
            {
                Primary = serverPrimary.PrimaryServer;
            }
        }
        public string ServerName { get; set; } = "localhost";
        public string ServerIP { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 5000;
        public Boolean Block { get; set; } = true;
        public Boolean Primary { get; set; } = false;
        public Boolean Active { get; set; } = false;
        public DateTime TimeAlive { get; set; }
        public ActionRequest Action { get; set; } = ActionRequest.OK;
        public ActionStatus ActionStatus { get; set; } = ActionStatus.OK;
        public void ServerNameUpdate()
        {
            ServerName = Dns.GetHostEntry(IPAddress.Parse(ServerIP)).HostName;
        }
    }
}
