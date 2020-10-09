using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TankLinkNotifierApi.Responses
{
    public class ApiResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("errorMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }

        [JsonProperty("parameterErrors", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> ParameterErrors { get; set; }
    }
}
