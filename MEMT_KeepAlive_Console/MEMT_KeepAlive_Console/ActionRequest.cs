using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEMT_KeepAlive
{
    public enum ActionRequest : int
    {
        OK = 0,
        ForceMigration = 1,
        ChangePrimary = 2,
        ServerBlocked = 3
    }
}
