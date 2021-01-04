using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TankUtilityCreateServiceBusTopic
{
    public class Payload
    {     
        public string TankId { get; set; }
        public string LongTankId { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public DateTime ReceivedOn { get; set; }
    }
}
