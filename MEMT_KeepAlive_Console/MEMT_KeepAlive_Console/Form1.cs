using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using MEMT_KeepAlive.Models;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using MEMT_KeepAlive;
namespace MEMT_KeepAlive_Console
{
  public partial class Form1 : Form
  {
    readonly ClsApplication _application = new ClsApplication();

    public Form1()
    {
      InitializeComponent();
    }

    private void CmdStart_Click(object sender, EventArgs e)
    {
      TmProcess.Enabled = true;
      TmTrees.Enabled = true;
    }

    private void CmdStop_Click(object sender, EventArgs e)
    {
      TmProcess.Enabled = false;
      TmTrees.Enabled = false;
      if (_application._pmeControl.StopPME())
      {
        _application._pmeControl._PMELocal.ActionStatus = ActionStatus.Sucess;
        _application._pmeControl._PMELocal.Active = false;
        _application._pmeControl.WriteStatusLocal();
      }
    }
    private void Form1_Load(object sender, EventArgs e)
    {
      TmProcess.Interval = (int) new System.Timers.Timer(Convert.ToInt32(ConfigurationManager.AppSettings["KeepAliveInterval"]) * 1000).Interval;
      TmTrees.Interval =  (int) new System.Timers.Timer(Convert.ToInt32(ConfigurationManager.AppSettings["TreeCopyInterval"]) * 60000).Interval;
    }

    private void TmProcess_Tick(object sender, EventArgs e)
    {
      try
      {
        // Intervalo padrao para execução do Timer --> valor lido do app.config
        BaseInfra _infra = new BaseInfra();
        _infra.TableExists();
        _application.KeepAliveVerify();
      }
      catch
      {
      }
    }

    private void TmTrees_Tick(object sender, EventArgs e)
    {
      try
      {
        _application._pmeControl.TreeCopy();
      }
      catch
      {
      }
    }
  }
}
