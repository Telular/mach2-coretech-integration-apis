
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Moment.Models
{
    public class MomentObjects
    {
        [JsonProperty("Moments")]
        public List<Moment> Moments { get; set; }
    }
}
