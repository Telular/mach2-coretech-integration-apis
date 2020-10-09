using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TankUtilityApiLib.Models
{
    public class Telemetry
    {
        [JsonProperty("attempt_no")]
        public int AttemptNo;

        [JsonProperty("band")]
        public int Band;

        [JsonProperty("cell_id")]
        public string CellId;

        [JsonProperty("chn")]
        public int Chn;

        [JsonProperty("fplmn")]
        public List<int> Fplmn;

        [JsonProperty("http_status_code")]
        public int HttpStatusCode;

        [JsonProperty("module_temp")]
        public double ModuleTemp;

        [JsonProperty("module_voltage")]
        public double ModuleVoltage;

        [JsonProperty("nbc")]
        public List<Nbc> Nbc;

        [JsonProperty("plmn")]
        public int Plmn;

        [JsonProperty("rat")]
        public string Rat;

        [JsonProperty("rsrp")]
        public double Rsrp;

        [JsonProperty("rsrq")]
        public double Rsrq;

        [JsonProperty("srxlev")]
        public double Srxlev;

        [JsonProperty("state")]
        public string State;

        [JsonProperty("tlm_time")]
        public int TlmTime;

        [JsonProperty("type")]
        public string Type;
    }
}
