using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEMT_KeepAlive
{
    public class ClsPMEService
    {
        public string[] ServiceNameStop { get; set; } = { "IONDiagnosticsAndUsage", "IONEW", "IONRS", "ManagedCircuitService", "PMLOGROUTER", "PMLXMLSubscriptions", "PMLXMLSubscriptionStore", "PowerCloudAgentService", "ProviderEngineHost", "DataServicesHost", "CoreServicesHost", "PMLION", "PMLLogServer", "PMLVIP_VIP.DDD", "PMLVIP_VIP.DEFAULT", "PMLVIP_VIP.PQADVISOR", "PMLQueryServer", "PMLNetman", "PMALARM", "PMLCM", "PMLMK", "PMLSS" };
        public string[] ServiceNameStart { get; set; } = { "IONDiagnosticsAndUsage", "IONEW", "IONRS", "ManagedCircuitService", "PMLCM", "PMLMK", "PMLOGROUTER", "PMLXMLSubscriptions", "PMLXMLSubscriptionStore", "PowerCloudAgentService", "CoreServicesHost", "DataServicesHost", "ProviderEngineHost", "PMLNetman", "PMLION", "PMLLogServer", "PMLVIP_VIP.DDD", "PMLVIP_VIP.DEFAULT", "PMLVIP_VIP.PQADVISOR", "PMALARM", "PMLQueryServer", "PMLSS" };
    }
}
