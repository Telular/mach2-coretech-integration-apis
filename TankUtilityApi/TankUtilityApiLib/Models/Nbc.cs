using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TankUtilityApiLib.Models
{
    public class Nbc
    {
        [JsonProperty("chn")]
        public int Chn;

        [JsonProperty("rat")]
        public string Rat;

        [JsonProperty("rsrp")]
        public double Rsrp;

        [JsonProperty("rsrq")]
        public double Rsrq;

        [JsonProperty("rssi")]
        public double Rssi;

        [JsonProperty("srxlev")]
        public int Srxlev;
    }
}
