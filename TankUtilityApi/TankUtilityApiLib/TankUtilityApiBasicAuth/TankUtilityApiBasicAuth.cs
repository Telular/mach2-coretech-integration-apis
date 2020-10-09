using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TankUtilityApiLib.TankUtilityApi;
using Newtonsoft.Json;

namespace TankUtilityApiLib.TankUtilityApiBasicAuth
{
    using Flurl;
    using Flurl.Http;
    using Flurl.Http.Configuration;
    using Common.Logging;
    using System.Configuration;
    using System.Runtime.Caching;
    using TankUtilityApiLib.Models;
    using System.Net;

    public class TankUtilityApiBasicAuth
    {
        #region Private fields

        private static readonly ILog _logger = LogManager.GetLogger("TankUtilityAPI");

        private static readonly IFlurlClientFactory _fluentClientFactory = new PerBaseUrlFlurlClientFactory();

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore };

        private static ApiOauthConfiguration _apiConfig;

        #endregion Private fields


        #region Constructors

        public TankUtilityApiBasicAuth()
        {
            try
            {
                _apiConfig = ConfigurationManager.GetSection("TankUtilityBasicAuthConfiguration") as ApiOauthConfiguration;

                if (_apiConfig == null)
                {
                    throw new NullReferenceException("No api configuration (Section: TankUtilityBasicAuthConfiguration) was retrieved from app config file.");
                }
                

                _fluentClientFactory.Get(_apiConfig.ApiUrl);

                _fluentClientFactory.Get(_apiConfig.AuthUrl);                
            }
            catch (Exception anException)
            {
                _logger.Error($"Tank Utility API Constructor Error:  {anException.Message} {anException?.InnerException?.Message}");
                _apiConfig = new ApiOauthConfiguration();
            }
        }

        public TankUtilityApiBasicAuth(ApiOauthConfiguration apiConfig)
        {
            try
            {
                _apiConfig = apiConfig;

                if (_apiConfig == null)
                {
                    throw new NullReferenceException("No api configuration (Section: TankUtilityConfiguration) was retrieved from app config file.");
                }

                _fluentClientFactory.Get(_apiConfig.ApiUrl);

                _fluentClientFactory.Get(_apiConfig.AuthUrl);
            }
            catch (Exception anException)
            {
                _logger.Error($"Tank Utility API Constructor Error:  {anException.Message} {anException?.InnerException?.Message}");
                _apiConfig = new ApiOauthConfiguration();
            }
        }

        #endregion Constructors     

        #region Public Methods

        public async Task<TankUtilityApiResponse> GetDeviceAsync(string tankId)
        {
            List<string> jsonErrors = new List<string>();

            TankUtilityApiResponse response = new TankUtilityApiResponse();

            // for this request method capture any de-serialization errora and mark them as "handled". 
            // get the error text and return it so we can see what's going on.
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                Error = (sender, eventArgs) =>
                {
                    eventArgs.ErrorContext.Handled = false;
                    jsonErrors.Add(eventArgs?.ErrorContext?.Error?.Message);
                }
            };

            try
            {                
                var aToken = await GetBearerAuthHeaderValue();

                System.Net.Http.HttpResponseMessage apiResponse = await _apiConfig.ApiUrl
                                    .AppendPathSegment("devices")
                                    .AppendPathSegment($"{tankId}")
                                    .SetQueryParam("token", $"{aToken}", true)
                                    .GetAsync();
                //.GetJsonAsync<DeviceResponse>();

                var apiResponseStr = await apiResponse.Content?.ReadAsStringAsync();
                response.ApiResponseStr = apiResponseStr;

                response.Data = JsonConvert.DeserializeObject<DeviceResponse>(apiResponseStr, _jsonSerializerSettings);

                response.Success = (apiResponse.StatusCode == HttpStatusCode.OK);                
                response.HttpStatusCode = (int)apiResponse.StatusCode;
                
            }
            catch (FlurlHttpTimeoutException anException)
            {

                // FlurlHttpTimeoutException derives from FlurlHttpException; catch here only
                // if you want to handle timeouts as a special case
                _logger.Error("Tank Utility Request Timed Out.", anException);

                response.Success = false;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorCode = (int)anException.Call?.HttpStatus;
                response.HttpStatusCode = (int)anException.Call?.HttpStatus;
                response.RequestProperties = new Dictionary<string, object>(anException?.Call?.Request.Properties);
                response.ErrorTitle = $"Get Device Request Timeout For - Tank Id {tankId} - Exception.";
            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)    
                _logger.Error("Tank Utility Request Exception.", anException);

                response.Success = false;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorCode = (int)anException.Call?.HttpStatus;
                response.HttpStatusCode = (int)anException.Call?.HttpStatus;
                response.RequestProperties = new Dictionary<string, object>(anException.Call?.Request.Properties);
                response.ErrorTitle = $"Get Device Request For - Tank Id {tankId} - Exception.";
            }
            catch (Exception anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)    
                _logger.Error("Tank Utility Exception.", anException);

                response.Success = false;
                response.JsonParsingErrors = jsonErrors;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";                                
                response.ErrorTitle = $"Get Device Request For - Tank Id {tankId} - Exception.";
            }

            return response;
        }

        public async Task<TankUtilityApiResponse> GetDeviceReadingsAsync(string tankId)
        {
            List<string> jsonErrors = new List<string>();

            TankUtilityApiResponse response = new TankUtilityApiResponse();

            // for this request method capture any de-serialization errora and mark them as "handled". 
            // get the error text and return it so we can see what's going on.
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                Error = (sender, eventArgs) =>
                {
                    eventArgs.ErrorContext.Handled = false;
                    jsonErrors.Add(eventArgs?.ErrorContext?.Error?.Message);
                }
            };

            try
            {
                var aToken = await GetBearerAuthHeaderValue();

                System.Net.Http.HttpResponseMessage apiResponse = await _apiConfig.ApiUrl
                                    .AppendPathSegment("readings")
                                    .AppendPathSegment($"{tankId}")
                                    .SetQueryParam("token", $"{aToken}", true)
                                    .GetAsync();
                //.GetJsonAsync<DeviceResponse>();

                var apiResponseStr = await apiResponse.Content?.ReadAsStringAsync();
                response.ApiResponseStr = apiResponseStr;                

                response.Data = JsonConvert.DeserializeObject<DeviceReadingsResponse>(apiResponseStr, _jsonSerializerSettings);

                response.Success = (apiResponse.StatusCode == HttpStatusCode.OK);
                response.HttpStatusCode = (int)apiResponse.StatusCode;

            }
            catch (FlurlHttpTimeoutException anException)
            {

                // FlurlHttpTimeoutException derives from FlurlHttpException; catch here only
                // if you want to handle timeouts as a special case
                _logger.Error("Tank Utility Request Timed Out.", anException);

                response.Success = false;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorCode = (int)anException.Call?.HttpStatus;
                response.HttpStatusCode = (int)anException.Call?.HttpStatus;
                response.RequestProperties = new Dictionary<string, object>(anException?.Call?.Request.Properties);
                response.ErrorTitle = $"Get Device Request Timeout For - Tank Id {tankId} - Exception.";
            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)    
                _logger.Error("Tank Utility Request Exception.", anException);

                response.Success = false;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorCode = (int)anException.Call?.HttpStatus;
                response.HttpStatusCode = (int)anException.Call?.HttpStatus;
                response.RequestProperties = new Dictionary<string, object>(anException.Call?.Request.Properties);
                response.ErrorTitle = $"Get Device Request For - Tank Id {tankId} - Exception.";
            }
            catch (Exception anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)    
                _logger.Error("Tank Utility Exception.", anException);

                response.Success = false;
                response.JsonParsingErrors = jsonErrors;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorTitle = $"Get Device Request For - Tank Id {tankId} - Exception.";
            }

            return response;
        }

        public async Task<TankUtilityApiResponse> GetDevicesAsync()
        {
            List<string> jsonErrors = new List<string>();

            TankUtilityApiResponse response = new TankUtilityApiResponse();

            try
            {                

                var aToken = await GetBearerAuthHeaderValue();

                System.Net.Http.HttpResponseMessage apiResponse = await _apiConfig.ApiUrl
                                    .AppendPathSegment("devices")
                                    .SetQueryParam("token", $"{aToken}", true)
                                    .GetAsync();
                

                var apiResponseStr = await apiResponse.Content?.ReadAsStringAsync();
                response.ApiResponseStr = apiResponseStr;

                response.Data = JsonConvert.DeserializeObject<DevicesResponse>(apiResponseStr, _jsonSerializerSettings);

                response.Success = (apiResponse.StatusCode == HttpStatusCode.OK);
                response.HttpStatusCode = (int)apiResponse.StatusCode;
            }
            catch (FlurlHttpTimeoutException anException)
            {

                // FlurlHttpTimeoutException derives from FlurlHttpException; catch here only
                // if you want to handle timeouts as a special case
                _logger.Error("Tank Utility Request Timed Out.", anException);

                response.Success = false;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorCode = (int)anException.Call?.HttpStatus;
                response.HttpStatusCode = (int)anException.Call?.HttpStatus;
                response.RequestProperties = new Dictionary<string, object>(anException?.Call?.Request.Properties);
                response.ErrorTitle = "Get Devices Request Timeout Exception.";
            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)                                
                _logger.Error("Tank Utility Request Exception.", anException);

                response.Success = false;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorCode = (int)anException.Call?.HttpStatus;
                response.HttpStatusCode = (int)anException.Call?.HttpStatus;
                response.RequestProperties = new Dictionary<string, object>(anException.Call?.Request.Properties);
                response.ErrorTitle = "Get Devices Request Exception.";
            }
            catch (Exception anException)
            {
                // any exception not related to the API web request e.g. JSON serialization errors, null args etc.   
                _logger.Error("Tank Utility Exception.", anException);

                response.Success = false;
                response.JsonParsingErrors = jsonErrors;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorTitle = $"Get Devices - Exception.";
            }

            return response;
        }

        public async Task<TankUtilityApiResponse> PatchDeviceConfigAsync(string tankId, Dictionary<string, object> tankConfiguration)
        {
            List<string> jsonErrors = new List<string>();

            TankUtilityApiResponse response = new TankUtilityApiResponse();

            try
            {
                var aToken = await GetBearerAuthHeaderValue();

                System.Net.Http.HttpResponseMessage apiResponse = await _apiConfig.ApiUrl
                                    .AppendPathSegment("devices")
                                    .AppendPathSegment($"{tankId}")
                                    .SetQueryParam("token", $"{aToken}", true)
                                    .PatchJsonAsync(tankConfiguration);                

                var apiResponseStr = await apiResponse.Content?.ReadAsStringAsync();
                response.ApiResponseStr = apiResponseStr;

                response.Data = apiResponseStr;

                response.Success = (apiResponse.StatusCode == HttpStatusCode.OK);
                response.HttpStatusCode = (int)apiResponse.StatusCode;

            }
            catch (FlurlHttpTimeoutException anException)
            {

                // FlurlHttpTimeoutException derives from FlurlHttpException; catch here only
                // if you want to handle timeouts as a special case
                _logger.Error("Tank Utility Request Timed Out.", anException);

                response.Success = false;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorCode = (int)anException.Call?.HttpStatus;
                response.HttpStatusCode = (int)anException.Call?.HttpStatus;
                response.RequestProperties = new Dictionary<string, object>(anException?.Call?.Request.Properties);
                response.ErrorTitle = $"Post Device Configuration Request Timeout For - Tank Id {tankId} - Exception.";
            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)                
                _logger.Error("Tank Utility Request Exception.", anException);

                response.Success = false;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorCode = (int)anException.Call?.HttpStatus;
                response.HttpStatusCode = (int)anException.Call?.HttpStatus;
                response.RequestProperties = new Dictionary<string, object>(anException.Call?.Request.Properties);                
                response.ErrorTitle = $"Post Device Configuration Request For - Tank Id {tankId} - Exception.";
            }
            catch (Exception anException)
            {
                // any exception not related to the API web request e.g. JSON serialization errors, null args etc.
                _logger.Error("Tank Utility Exception.", anException);

                response.Success = false;
                response.JsonParsingErrors = jsonErrors;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorTitle = $"Get Device Request For - Tank Id {tankId} - Exception.";
            }

            return response;
        }

            #endregion Public Methods

            #region Private Methods


        private static async Task<OauthTokenResponse> GetOathTokenResponse()
        {
            OauthTokenResponse response = new OauthTokenResponse();

            var username = _apiConfig.AuthClientId;
            var password = _apiConfig.AuthClientSecret;            

            try
            {

                System.Net.Http.HttpResponseMessage apiResponse = await _apiConfig.AuthUrl
                                .WithBasicAuth(username, password)
                                .GetAsync();
                

                var apiResponseStr = await apiResponse.Content?.ReadAsStringAsync();
                

                var apiToken = JsonConvert.DeserializeObject<Dictionary<string,string>>(apiResponseStr, _jsonSerializerSettings);

                response.expires_in = 24 * 60 * 60;
                response.access_token = apiToken.ContainsKey("token") ? apiToken["token"] : string.Empty;
            }
            catch (FlurlHttpTimeoutException anException)
            {
                // FlurlHttpTimeoutException derives from FlurlHttpException; catch here only
                // if you want to handle timeouts as a special case                
                throw new TankUtilityApiException((int)anException.Call.HttpStatus, anException.Message, anException);
            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)
                throw new TankUtilityApiException((int)anException.Call.HttpStatus, anException.Message, anException);
            }
            catch (Exception anException)
            {
                // Garden variety exception.  Flag it as 500 server error.
                throw new TankUtilityApiException(500, anException.Message, anException);
            }

            return response;
        }

        public static async Task<string> GetBearerAuthHeaderValue()
        {
            //try to get token from cache

            if (!(MemoryCache.Default["bearer_token"] is OauthTokenResponse tokenResponse))
            {
                // nothing in cache, get a new token
                tokenResponse = await GetOathTokenResponse();

                if (tokenResponse != null)
                {
                    MemoryCache.Default["bearer_token"] = tokenResponse;
                }

            }
            else
            {
                // got something is it expired or about to be expired?
                if (tokenResponse.ExpiresOn <= DateTime.UtcNow.AddSeconds(60))
                {

                    tokenResponse = await GetOathTokenResponse();
                    MemoryCache.Default["bearer_token"] = tokenResponse;
                }
            }
            return $"{(tokenResponse != null ? tokenResponse.access_token : string.Empty)}";
        }
        #endregion Private Methods
    }


}
