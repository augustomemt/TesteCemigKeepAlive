using MEMT_KeepAlive.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEMT_KeepAlive
{
    class ClsApplication
    {
        public ClsPMEControl _pmeControl { get; set; } = new ClsPMEControl();
        BaseContext _context = new BaseContext();
        int _reset = 0;
        bool _MyRequest = true;
        internal bool UpdateStatus()
        {
            return _pmeControl.UpdateStatus();
        }
        internal void WriteStatusLocal()
        {
            _pmeControl.WriteStatusLocal();
        }
        internal void KeepAliveVerify()
        {
            if (UpdateStatus())
            {
                switch (_pmeControl._PMELocal.Action)
                {
                    case ActionRequest.ChangePrimary:
                        ChangePrimary();
                        break;
                    case ActionRequest.ForceMigration:
                        ForceMigration();
                        break;
                    case ActionRequest.OK:
                        switch (_pmeControl._PMERemote.Action)
                        {
                            case ActionRequest.ChangePrimary:
                                ChangePrimary();
                                break;
                            case ActionRequest.ForceMigration:
                                ForceMigration();
                                break;
                            case ActionRequest.OK:
                                KeepAliveSend();
                                break;
                            case ActionRequest.ServerBlocked:
                                if (!_pmeControl._PMELocal.Active && !_pmeControl._PMERemote.Active)
                                {
                                   // ClsLog.AddLog("KeepAlive (PME Remote BLOCK) - StartPME Local-->" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                                    _pmeControl.StartPME();
                                }
                                AtualizaStatusLocal();
                                break;
                        }
                        break;
                    case ActionRequest.ServerBlocked:
                        break;
                }
                WriteStatusLocal();
            }
            else
            {
                //ClsLog.AddLog("KeepAlive (Erro Conexão BD) -->" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
            }
        }
        private void AtualizaStatusLocal()
        {
            if (_pmeControl._PMELocal.ActionStatus == ActionStatus.Sucess || _pmeControl._PMELocal.ActionStatus == ActionStatus.Fail)
            {
                if (_reset > 3)
                {
                    _reset = 0;
                    _pmeControl._PMELocal.ActionStatus = ActionStatus.OK;
                }
                else
                {
                    _reset++;
                }
            }
        }
        private void ForceMigration()
        {
            switch (_pmeControl._PMELocal.Action)
            {
                case ActionRequest.OK: //pedido de force recebido pme remoto
                    if (_pmeControl._PMERemote.ActionStatus == ActionStatus.Request)
                    {
                        _MyRequest = false;
                        _pmeControl._PMELocal.ActionStatus = ActionStatus.Waiting;
                        _pmeControl._PMELocal.Action = ActionRequest.ForceMigration;
                    }
                    if (_pmeControl._PMERemote.ActionStatus == ActionStatus.OK)
                    {
                        _pmeControl._PMELocal.ActionStatus = ActionStatus.OK;
                        _pmeControl._PMELocal.Action = ActionRequest.OK;
                        _MyRequest = true;
                    }
                    break;
                case ActionRequest.ForceMigration:
                    switch (_pmeControl._PMELocal.ActionStatus)
                    {
                        case ActionStatus.Request:
                            if (_pmeControl._PMERemote.ActionStatus == ActionStatus.Waiting)
                            {
                                _pmeControl._PMELocal.ActionStatus = ActionStatus.Stoping;
                                WriteStatusLocal();
                                if (_pmeControl.StopPME())
                                {
                                    _pmeControl._PMELocal.ActionStatus = ActionStatus.Waiting;
                                }
                                else
                                {
                                    _pmeControl._PMELocal.ActionStatus = ActionStatus.Starting;
                                    WriteStatusLocal();
                                    if (_pmeControl.StartPME())
                                    {
                                        _pmeControl._PMELocal.ActionStatus = ActionStatus.OK;
                                        _pmeControl._PMELocal.Action = ActionRequest.OK;
                                    }
                                    //else
                                    //{
                                    //  //tratar falha ao parar e iniciou o pme local novamente (FALHA AO INICIAR) //DEFINIR LOGICA
                                    //}
                                }
                            }
                            _MyRequest = true;
                            break;
                        case ActionStatus.Waiting:
                            if (_pmeControl._PMERemote.ActionStatus == ActionStatus.Waiting)
                            {
                                if (!_MyRequest) // pedido Remoto  // pme remoto parou
                                {
                                    _pmeControl._PMELocal.ActionStatus = ActionStatus.Starting;
                                    WriteStatusLocal();
                                    if (_pmeControl.StartPME())
                                    {
                                        _pmeControl._PMELocal.ActionStatus = ActionStatus.Sucess;
                                    }
                                    else
                                    {
                                        WriteStatusLocal();
                                        _pmeControl._PMELocal.ActionStatus = ActionStatus.Stoping;
                                        WriteStatusLocal();
                                        _pmeControl.StopPME();
                                        _pmeControl._PMELocal.ActionStatus = ActionStatus.Fail;
                                    }
                                }
                            }
                            if (_pmeControl._PMERemote.ActionStatus == ActionStatus.Sucess)
                            {
                                _pmeControl._PMELocal.ActionStatus = ActionStatus.OK;
                            }
                            if (_pmeControl._PMERemote.ActionStatus == ActionStatus.Fail)
                            {
                                _pmeControl._PMELocal.ActionStatus = ActionStatus.Rollback;
                                //iniciar pme local novamente
                            }
                            break;
                        case ActionStatus.Sucess:
                            if (_pmeControl._PMERemote.ActionStatus == ActionStatus.OK)
                            {
                                _pmeControl._PMELocal.ActionStatus = ActionStatus.OK;
                            }
                            break;
                        case ActionStatus.Fail:
                            if (_pmeControl._PMERemote.ActionStatus == ActionStatus.Rollback)
                            {
                                _pmeControl._PMELocal.ActionStatus = ActionStatus.Waiting;
                            }
                            break;
                        case ActionStatus.OK:
                            _pmeControl._PMELocal.Action = ActionRequest.OK;
                            break;
                    }
                    break;
            }
        }
        private void ChangePrimary()
        {
            switch (_pmeControl._PMELocal.Action)
            {
                case ActionRequest.OK: //pedido de change recebido pme remoto
                    if (_pmeControl._PMELocal.ActionStatus == ActionStatus.Sucess)
                    {
                        _pmeControl._PMELocal.ActionStatus = ActionStatus.OK;
                    }
                    else
                    {
                        _pmeControl._PMELocal.Action = ActionRequest.ChangePrimary;
                        _pmeControl._PMELocal.Primary = !_pmeControl._PMELocal.Primary;
                        _pmeControl._PMELocal.ActionStatus = ActionStatus.Sucess;
                    }
                    break;
                case ActionRequest.ChangePrimary:
                    if (_pmeControl._PMERemote.ActionStatus == ActionStatus.Sucess)
                    {
                        if (_pmeControl._PMELocal.ActionStatus == ActionStatus.Sucess)
                        {
                            _pmeControl._PMELocal.ActionStatus = ActionStatus.OK;
                        }
                        if (_pmeControl._PMELocal.ActionStatus == ActionStatus.Request)
                        {
                            _pmeControl._PMELocal.Primary = !_pmeControl._PMELocal.Primary;
                            _pmeControl._PMELocal.ActionStatus = ActionStatus.Sucess;
                        }
                    }
                    if (_pmeControl._PMERemote.ActionStatus == ActionStatus.OK)
                    {
                        if (_pmeControl._PMELocal.ActionStatus == ActionStatus.Sucess)
                        {
                            _pmeControl._PMELocal.ActionStatus = ActionStatus.OK;
                        }
                        if (_pmeControl._PMELocal.ActionStatus == ActionStatus.OK)
                        {
                            _pmeControl._PMELocal.Action = ActionRequest.OK;
                        }
                    }
                    break;
            }
        }
        private void KeepAliveSend()
        {
            UpdateStatus(); //Confirmação de Status Local e Remoto antes de executar ações.
            switch (_pmeControl._PMERemote.ActionStatus)
            {
                case ActionStatus.Down:
                    if (!_pmeControl._PMELocal.Active) //PME Remote DOWN e PME Local Parado - Start PME Local
                    {
                        //ClsLog.AddLog("KeepAlive (PME Remote DOWN) - StartPME Local-->" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                        _pmeControl.StartPME();
                    }
                    AtualizaStatusLocal();
                    break;
                case ActionStatus.Starting:
                    //ClsLog.AddLog("KeepAlive (PME Remote STARTING) - StopPME Local-->" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                    _pmeControl.StopPME();
                    break;
                case ActionStatus.Block:
                    break;
                default: //PME Remote <> DOWN e STARTING
                    AtualizaStatusLocal();
                    if (!_pmeControl._PMERemote.Active && !_pmeControl._PMELocal.Active) //2 PME Parados e Local Primary - Start PME Local
                    {
                        if (_pmeControl._PMELocal.Primary)
                        {
//ClsLog.AddLog("KeepAlive (2 PMEs Parados e Primario) - StartPME Local-->" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                            _pmeControl.StartPME();
                        }
                    }
                    if (_pmeControl._PMERemote.Active && _pmeControl._PMELocal.Active) //2 PME Ativos e Local  not Primary - StopPME Local
                    {
                        if (!_pmeControl._PMELocal.Primary)
                        {
                            //.AddLog("KeepAlive (2 PMEs Ativos e Primario) - StartPME Local-->" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss" + "\n"));
                            _pmeControl.StopPME();
                        }
                    }
                    break;
            }
        }
    }
}
