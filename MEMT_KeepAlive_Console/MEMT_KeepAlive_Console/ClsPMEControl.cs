using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEMT_KeepAlive
{
    class ClsPMEControl
    {
        public ClsPME _pme { get; set; } = new ClsPME();
        public ClsPMEServer _PMELocal { get; set; } = new ClsPMEServer(ConfigurationManager.AppSettings["LocalServerIP"], Convert.ToBoolean(ConfigurationManager.AppSettings["Primary"]));
        public ClsPMEServer _PMERemote { get; set; } = new ClsPMEServer(ConfigurationManager.AppSettings["RemoteServerIP"]);
        internal bool CheckConnection()
        {
            return _pme.CheckPMEConnection();
        }
        internal bool WriteStatusLocal()
        {
            //if (_PMELocal.Primary == _PMERemote.Primary)
            //{
            //    _PMELocal.Primary = Convert.ToBoolean(ConfigurationManager.AppSettings["Primary"]);

            //}
            if (_pme.WriteStatus(_PMELocal))
            {
                _PMELocal = _pme.ReadStatus("LOCAL");
                return true;
            }
            return false;
        }
        internal bool StopPME()
        {
            _PMELocal.ActionStatus = ActionStatus.Stoping;
            WriteStatusLocal();
            if (_pme.StopServices(0))
            {
               // ClsLog.AddLog("StopPME (Sucesso)-->" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                _PMELocal.ActionStatus = ActionStatus.Sucess;
                return true;
            }
            else
            {
               // ClsLog.AddLog("StopPME (Falha)-->" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                _PMELocal.ActionStatus = ActionStatus.Fail;
                return false;
            }
        }
        internal bool StartPME()
        {
            _PMELocal.ActionStatus = ActionStatus.Starting;
            WriteStatusLocal();
            if (_pme.StartServices(0, _PMERemote.ServerName, _PMELocal.ServerName))
            {
                //ClsLog.AddLog("StartPME (Sucesso)-->" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                _PMELocal.ActionStatus = ActionStatus.Sucess;
                return true;
            }
            else
            {
                //ClsLog.AddLog("StartPME (Falha)-->" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                _PMELocal.ActionStatus = ActionStatus.Fail;
                return false;
            }
        }
        internal bool ReadStatus(String LocalRemote)
        {
            if (LocalRemote == "LOCAL" || LocalRemote == "REMOTE")
            {
                if (LocalRemote == "LOCAL")
                {
                    _PMELocal = _pme.ReadStatus(LocalRemote);
                }
                else
                {
                    _PMERemote = _pme.ReadStatus(LocalRemote);
                }
                return true;
            }
            return false;
        }
        internal bool UpdateStatus()
        {
            _PMERemote = _pme.ReadStatus("REMOTE");
            if (WriteStatusLocal())
            {
                
                if (DateTime.Now.Subtract(_PMERemote.TimeAlive).TotalSeconds >= Convert.ToInt32(ConfigurationManager.AppSettings["KeepAliveErrorCount"]) * Convert.ToInt32(ConfigurationManager.AppSettings["KeepAliveInterval"]))
                {
                    if (_PMERemote.Action == ActionRequest.OK)
                    {
                        _PMERemote.ActionStatus = ActionStatus.Down;
                    }

                }
                return true;
            }
            else
            {
                return false;
            }
        }

        internal bool TreeCopy()
        {
            try
            {
                var pathRemote = ConfigurationManager.AppSettings["TreeRemote"].Replace("server_Ip", ConfigurationManager.AppSettings["RemoteServerIP"]);
                var pathLocal = ConfigurationManager.AppSettings["TreeLocal"].Replace("client_Ip", ConfigurationManager.AppSettings["LocalServerIP"]);
                if (_PMELocal.Active)
                {
                    using (new NetworkConnection.NetworkConnection(pathRemote, new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Tree_userName"], ConfigurationManager.AppSettings["Tree_password"])))
                    {
                        using (new NetworkConnection.NetworkConnection(pathLocal, new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Tree_userName"], ConfigurationManager.AppSettings["Tree_password"])))
                        {
                            CopyAll(pathLocal, pathRemote, _PMELocal.ServerName.Split('.')[0]);
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        internal void CopyAll(string source, string target, string servername)
        {
            DirectoryInfo sourceDir = new DirectoryInfo(source);
            DirectoryInfo targetDir = new DirectoryInfo(target);

            foreach (FileInfo fi in sourceDir.GetFiles())
            {
                if (!(fi.Name.Contains("VIP") || fi.Name.Contains(servername)))
                {
                   // ClsLog.AddLog("TreeCopy Files --> " + fi.Name + "\n");
                    fi.CopyTo(Path.Combine(targetDir.FullName, fi.Name), true);

                }
            }
        }
    }
}
