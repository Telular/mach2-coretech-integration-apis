
using Newtonsoft.Json;

namespace TankNotifierApiNet.Models
{
    public class TankUtilityCacheDevice
    {
        [JsonProperty(PropertyName = "shortTankId")]
        public string ShortTankId { get; set; }

        [JsonProperty(PropertyName = "longTankId")]
        public string LongTankId { get; set; }

        [JsonProperty(PropertyName = "isValidTankId")]
        public bool IsValidTankId { get; set; }

        [JsonProperty(PropertyName = "errorMessage")]
        public string ErrorMessage { get; set; }

    }
}