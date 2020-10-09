using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib
{
    public class BaseApiResponse
    {
        [JsonProperty("success", NullValueHandling = NullValueHandling.Ignore)]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "result", NullValueHandling = NullValueHandling.Ignore)]
        public string Result { get; set; }

        [JsonProperty("errorMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }

        [JsonProperty("errorCode", NullValueHandling = NullValueHandling.Ignore)]
        public int? ErrorCode { get; set; }

        [JsonProperty("errorTitle", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorTitle { get; set; }

        [JsonProperty("statusCode", NullValueHandling = NullValueHandling.Ignore)]
        public int? HttpStatusCode { get; set; }

        [JsonProperty("parameterErrors", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ParameterErrors { get; set; }

        [JsonProperty("requestProperties", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> RequestProperties { get; set; }

        [JsonProperty("apiResponseStr", NullValueHandling = NullValueHandling.Ignore)]
        public string ApiResponseStr { get; set; }

        [JsonProperty("jsonParsingErrors", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> JsonParsingErrors { get; set; }
    }
}
