using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetLinkBulkProvisioner
{
    public class AssetLinkParams
    {
        
        public const string TopLevelGroup = "Skybitz";
        
        public const string DebugProvisionPlan = "SBD Track 7.6 KB";
        // The 3.2KB plan for Asset Link has the Billing Plan of “SVCSD-irb3200    3200B” linked to it for Telular.  This is the plan that should be used for PROD devices.
        // NOTE:  This plan is what gets used by our Carrier API as the default "plan" if "No Plan" is specified when you "activate" an stxx-al-iridium device.
        public const string DefaultProvisionPlan = "SBD Track 3.2 KB";
    }
}
