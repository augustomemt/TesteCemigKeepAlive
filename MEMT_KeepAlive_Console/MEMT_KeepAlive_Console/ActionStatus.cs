using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEMT_KeepAlive
{
    public enum ActionStatus
    {
        OK = 0,  //Conexão OK //Banco OK // Serviços PME OK
        Request = 1, //Servidor Enviou requisição 
        Running = 2,
        Fail = 3,
        Sucess = 4,
        Block = 5,
        Waiting = 6,
        Rollback = 7,
        Changing = 8,
        Down = 9, //Sem Conexão
        Connected = 10, //Tem Conexão , Serviços PME Parado
        Unknown = 11,//pré-conexao
        Force = 12,
        Starting = 13,
        Stoping = 14
    }
}
