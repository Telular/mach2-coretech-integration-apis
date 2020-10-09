using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetLinkGlobalTests
{
    public class AssetLinkGlobalTestParams
    {
        public const string GoodUrn = "urn:dev:os:test:000001";
        public const string GoodEsn = "300234065511460";
        public const string GoodEsnToo = "300234068885830";
        public const string BadEsn = "010101010101010";
        public const string SpiFlashESN = "300234068885830";
        public const string SpiFlashESN_MReuter = "300234068988510 ";
        public const string GoodESNForMoData = "300234068989490";
        public const string GoodGroup = "Skybitz";
        public const string GoodMomentId = "1234567";
        public const string GoodMomentIdForMoData = "35538375";
        public const string CarrierName = "Test-AL-Iridium";
        public const string ProvisionEsn = "300234068981840";
        public const string ProvisionPlan = "SBD Track 7.6 KB";
        // The 3.2KB plan for Asset Link has the Billing Plan of “SVCSD-irb3200    3200B” linked to it for Telular.  This is the plan that should be used for PROD devices.
        // NOTE:  This plan is what gets used our Carrier API as the default "plan" if "No Plan" is specified when you fire the "provision" command.
        public const string AssetLinkDefaultProvisionPlan = "SBD Track 3.2 KB";  
    }
}
