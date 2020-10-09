using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TankLinkNotifierApi.Models
{
    public class DeviceNew
    {
        [JsonProperty("device_id")]
        public string DeviceId;

        [JsonProperty("short_device_id")]
        public string ShortDeviceId;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("address")]
        public string Address;

        [JsonProperty("account_id")]
        public string AccountId;

        [JsonProperty("fuel_type")]
        public string FuelType;

        [JsonProperty("status")]
        public string Status;

        [JsonProperty("capacity")]
        public int Capacity;

        [JsonProperty("orientation")]
        public string Orientation;

        [JsonProperty("consumption_types")]
        public string ConsumptionTypes;

        [JsonProperty("battery_warn")]
        public bool BatteryWarn;

        [JsonProperty("battery_crit")]
        public bool BatteryCrit;

        [JsonProperty("reading_interval")]
        public int ReadingInterval;

        [JsonProperty("transmission_interval")]
        public int TransmissionInterval;

        [JsonProperty("threshold_1")]
        public int Threshold1;

        [JsonProperty("threshold_2")]
        public int Threshold2;

        [JsonProperty("lastReading")]
        public LastReading LastReading;

        [JsonProperty("telemetry")]
        public List<TelemetryNew> Telemetry;
    }
}
