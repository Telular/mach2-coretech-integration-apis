using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TankUtilityApiLib.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    

   

      

   

    

    public class Root
    {
        [JsonProperty("device")]
        public DeviceNew Device;
    }


}
