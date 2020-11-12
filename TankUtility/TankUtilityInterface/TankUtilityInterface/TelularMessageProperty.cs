using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceActor.Common.DataContracts
{
    public class TelularMessageProperty
    {
        public string id { get; set; }
        public string originationsourcetype { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string datatype { get; set; }
        public string parentsequenceid { get; set; }
        public string sequenceid { get; set; }
        public string parentid { get; set; }
    }
}
