using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankLinkNotifierApi.Models
{
    public class LastReading
    {
        public double Tank { get; set; }
        public double Temperature { get; set; }
        public long Time { get; set; }
        public DateTime Time_iso { get; set; }
    }
}
