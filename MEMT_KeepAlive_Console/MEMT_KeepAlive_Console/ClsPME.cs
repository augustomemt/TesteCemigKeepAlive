using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MEMT_KeepAlive
{
    public class ClsPME
    {
        SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["IONData"].ConnectionString);
        ClsPMEService _pmeService = new ClsPMEService();
        bool result;
        internal bool CheckPMELocal()
        {
            try
            {
                return (CheckPMEConnection() && CheckPMEServices());
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckPMEConnection()
        {
            try
            {
                sqlConn.Open();
                sqlConn.Close();
                //ClsLog.AddLog("CheckPMEConnection (Sucesso)-->" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                return true;
            }
            catch (Exception)
            {
                sqlConn.Close();
                //ClsLog.AddLog("CheckPMEConnection (Falha)-->" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                if (CheckPMEServices())
                {
                    StopServices(0);
                }
                return false;
            }
        }
        public bool CheckPMEServices()
        {
            bool _result = false;
            try
            {
                foreach (string _service in _pmeService.ServiceNameStop)
                {
                    PowerShell ps2 = PowerShell.Create().AddCommand("get-service").AddParameter("Name", _service);
                    Collection<PSObject> StatusService = ps2.Invoke();
                    if ((ServiceControllerStatus)StatusService[0].Members["Status"].Value == ServiceControllerStatus.Running)
                    {
                        //ClsLog.AddLog("CheckPMEService "+_service+" (Running)--> " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                        _result = true;
                    }
                    else
                    {
                        //ClsLog.AddLog("CheckPMEService " + _service + " (Stopped)--> " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                    }
                }
            }
            catch
            {
            }
            return _result;
        }
        public bool StopServices(int nchamada)
        {
            ChangeManual();
            if (nchamada == 0)
            {
                result = true;
            }
            foreach (string _service in _pmeService.ServiceNameStop)
            {
                try
                {
                    PowerShell ps2 = PowerShell.Create().AddCommand("get-service").AddParameter("Name", _service);
                    Collection<PSObject> StatusService = ps2.Invoke();
                    if ((ServiceControllerStatus)StatusService[0].Members["Status"].Value == ServiceControllerStatus.Running)
                    {
                        PowerShell StartService = PowerShell.Create();
                        StartService.AddCommand("Stop-Service").AddParameter("Name", _service).AddParameter("Force");
                        //ClsLog.AddLog("StopService " + _service + " (STOP)--> " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                        StartService.Invoke();
                    }
                }
                catch (Exception)
                {
                }
            }
            try
            {
                PowerShell ConfirmService = PowerShell.Create().AddCommand("get-service").AddParameter("DisplayName", "ION*");
                Collection<PSObject> resultsN = ConfirmService.Invoke();
                foreach (PSObject _service in resultsN)
                {
                    if (_service != null)
                    {
                        if (Array.Exists(_pmeService.ServiceNameStop, element => element == (string)_service.Members["Name"].Value) && (ServiceControllerStatus)_service.Members["Status"].Value == ServiceControllerStatus.Running)
                        {
                            if (nchamada <= 10)
                            {
                                if (result)
                                {
                                    //ClsLog.AddLog("StopService NEW (" + (nchamada + 1) + ") " + _service + " (STOP)--> " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                                    StopServices(nchamada++);
                                }
                            }
                            else
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public bool ChangeManual()
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddScript("Get-Service | Where-Object {$_.DisplayName -like 'ION*' -and $_.Status -eq 'Stopped' -and $_.StartType -like 'Manual'}|Set-Service -StartupType 'manual'");
                try
                {
                   // ClsLog.AddLog("ChangeManual: Passando Serviços para Manual --> " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                    PowerShellInstance.Invoke();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }
        public bool ChangeServerNameBase(string oldName, string newName)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("sp_MEMT_ChangeServerName", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@oldName", SqlDbType.VarChar)).Value = oldName.Split('.')[0];
                cmd.Parameters.Add(new SqlParameter("@newName", SqlDbType.VarChar)).Value = newName.Split('.')[0];
                sqlConn.Open();
                cmd.ExecuteNonQuery();
                sqlConn.Close();
               // ClsLog.AddLog("ChangeServerName: OldName(" + oldName.Split('.')[0] + ")/NewName(" + newName.Split('.')[0] + ") --> " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                return true;
            }
            catch (Exception)
            {
                sqlConn.Close();
                return false;
            }
        }
        public bool StartServices(int nchamada, string oldName, string newName)
        {
            if (ChangeServerNameBase(oldName, newName))
            {
                if (nchamada == 0)
                {
                    result = true;
                }
                try
                {
                    foreach (string _service in _pmeService.ServiceNameStart)
                    {
                        PowerShell ps2 = PowerShell.Create().AddCommand("get-service").AddParameter("Name", _service);
                        Collection<PSObject> StatusService = ps2.Invoke();
                        if ((ServiceControllerStatus)StatusService[0].Members["Status"].Value == ServiceControllerStatus.Stopped) //Verifica Serviço se está parado, se sim Inicia Serviço
                        {
                            PowerShell StartService = PowerShell.Create();
                            StartService.AddCommand("Start-Service").AddParameter("Name", _service);
                            Console.WriteLine("Iniciando Serviço:{0}", _service);
                            StartService.Invoke();
                            ///ClsLog.AddLog("StartService " + _service + " (START)--> " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                        }
                    }
                }
                catch (Exception)
                {

                }
                try
                {
                    PowerShell ConfirmService = PowerShell.Create().AddCommand("get-service").AddParameter("DisplayName", "ION*");  //Verificação se todos os Serviços estao Rodando
                    Collection<PSObject> resultsN = ConfirmService.Invoke();
                    foreach (PSObject _service in resultsN)
                    {
                        if (_service != null)
                        {
                            if (Array.Exists(_pmeService.ServiceNameStart, element => element == (string)_service.Members["Name"].Value) && (ServiceControllerStatus)_service.Members["Status"].Value == ServiceControllerStatus.Stopped)
                            {
                                if (nchamada <= 10)
                                {
                                    if (result)
                                    {
                                       // ClsLog.AddLog("StartService NEW (" + (nchamada + 1) + ") " + _service + " (START)--> " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                                        StartServices(nchamada++, oldName, newName);
                                    }
                                }
                                else
                                {
                                    result = false;
                                }
                            }
                        }
                    }

                }
                catch (Exception)
                {
                }
                return result;
            }
            else
            {
                return false;
            }
        }
        internal bool WriteStatus(ClsPMEServer _PMEServer)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("sp_MEMT_InsertStatus", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ServerIP", SqlDbType.VarChar)).Value = _PMEServer.ServerIP;
                cmd.Parameters.Add(new SqlParameter("@ServerPort", SqlDbType.Int)).Value = _PMEServer.ServerPort;
                if (_PMEServer.ServerName == "localhost")
                {
                    try
                    {
                        _PMEServer.ServerNameUpdate();
                    }
                    catch (Exception)
                    {
                    }
                }
                cmd.Parameters.Add(new SqlParameter("@ServerName", SqlDbType.VarChar)).Value = _PMEServer.ServerName;
                cmd.Parameters.Add(new SqlParameter("@ServiceAlive", SqlDbType.Bit)).Value = CheckPMELocal();
                cmd.Parameters.Add(new SqlParameter("@ServiceAliveTime", SqlDbType.DateTime)).Value = DateTime.Now;
                cmd.Parameters.Add(new SqlParameter("@BlockServer", SqlDbType.Bit)).Value = _PMEServer.Block;
                cmd.Parameters.Add(new SqlParameter("@PrimaryServer", SqlDbType.Bit)).Value = _PMEServer.Primary;
                cmd.Parameters.Add(new SqlParameter("@Action", SqlDbType.Char)).Value = Convert.ToInt32(_PMEServer.Action);
                cmd.Parameters.Add(new SqlParameter("@ActionStatus", SqlDbType.Int)).Value = Convert.ToInt32(_PMEServer.ActionStatus);
                sqlConn.Open();
                cmd.ExecuteNonQuery();
                sqlConn.Close();
                return true;
            }
            catch (Exception)
            {
                sqlConn.Close();
                return false;
            }
        }
        internal ClsPMEServer ReadStatus(string LocalRemote)
        {
            ClsPMEServer _PMEServer = new ClsPMEServer();
            SqlCommand cmd;
            if (LocalRemote == "LOCAL")
            {
                cmd = new SqlCommand("SELECT * FROM KeepAlive WHERE ServerIP = '" + ConfigurationManager.AppSettings["LocalServerIP"] + "'", sqlConn);
            }
            else
            {
                cmd = new SqlCommand("SELECT * FROM KeepAlive WHERE ServerIP = '" + ConfigurationManager.AppSettings["RemoteServerIP"] + "'", sqlConn);
            }
            try
            {
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    _PMEServer.ServerIP = reader["ServerIP"].ToString();
                    _PMEServer.ServerPort = Convert.ToInt32(reader["ServerPort"].ToString());
                    _PMEServer.ServerName = reader["ServerName"].ToString();
                    _PMEServer.Block = Convert.ToBoolean(reader["ServerBlock"].ToString());
                    _PMEServer.Active = Convert.ToBoolean(reader["ServiceAlive"].ToString());
                    _PMEServer.TimeAlive = Convert.ToDateTime(reader["ServiceAliveTime"].ToString());
                    _PMEServer.Primary = Convert.ToBoolean(reader["PrimaryServer"].ToString());
                    _PMEServer.Action = (ActionRequest)Enum.Parse(typeof(ActionRequest), reader["Action"].ToString());
                    _PMEServer.ActionStatus = (ActionStatus)Enum.Parse(typeof(ActionStatus), reader["ActionStatus"].ToString());
                }
                sqlConn.Close();
                return _PMEServer;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
