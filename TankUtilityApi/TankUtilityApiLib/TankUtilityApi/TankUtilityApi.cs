using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TankUtilityApiLib.TankUtilityApi
{
    using Flurl;
    using Flurl.Http;
    using Flurl.Http.Configuration;
    using Common.Logging;
    using System.Configuration;
    using System.Web;
    using System.Runtime.Caching;
    using TankUtilityApiLib.Models;
    using System.Net;
    using Newtonsoft.Json;

    public class TankUtilityApi
    {
        #region Private fields

        private static readonly ILog _logger = LogManager.GetLogger("TankUtilityAPI");

        private static readonly IFlurlClientFactory _fluentClientFactory = new PerBaseUrlFlurlClientFactory();

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore };

        private static ApiOauthConfiguration _apiConfig;

        private static readonly object _singletonLock = new object();

        /// <summary>
        /// Our singleton instance
        /// </summary>
        private static TankUtilityApi _singletonApi;

        #endregion Private fields


        #region Constructors

        public TankUtilityApi()
        {
            try
            {
                _apiConfig = ConfigurationManager.GetSection("TankUtilityConfiguration") as ApiOauthConfiguration;

                if (_apiConfig == null)
                {
                    throw new NullReferenceException("No api configuration (Section: TankUtilityConfiguration) was retrieved from app config file.");
                }

                _fluentClientFactory.Get(_apiConfig.AuthUrl);

                _fluentClientFactory.Get(_apiConfig.ApiUrl);

            }
            catch (Exception anException)
            {
                _logger.Error($"Tank Utility API Constructor Error:  {anException.Message} {anException?.InnerException?.Message}");
                _apiConfig = new ApiOauthConfiguration();
            }
        }

        public TankUtilityApi(ApiOauthConfiguration apiConfig)
        {
            try
            {
                _apiConfig = apiConfig;

                if (_apiConfig == null)
                {
                    throw new NullReferenceException("No api configuration (Section: TankUtilityConfiguration) was retrieved from app config file.");
                }

                _fluentClientFactory.Get(_apiConfig.AuthUrl);

                _fluentClientFactory.Get(_apiConfig.ApiUrl);
                                               
            }
            catch (Exception anException)
            {
                _logger.Error($"Tank Utility API Constructor Error:  {anException.Message} {anException?.InnerException?.Message}");
                _apiConfig = new ApiOauthConfiguration();
            }
        }

        #endregion Constructors     

        #region Singleton_Access

        /// <summary>
        /// Access to the singleton instance
        /// </summary>
        public static TankUtilityApi Instance
        {
            get
            {
                if (_singletonApi == null)
                {
                    lock (_singletonLock)
                    {
                        if (_singletonApi == null)
                        {
                            _singletonApi = new TankUtilityApi();
                        }
                    }
                }

                return _singletonApi;
            }
        }

        #endregion Singleton_Access

        #region Public Methods
        public async Task<TankUtilityApiResponse> GetDevicesAsync()
        {
            TankUtilityApiResponse response = new TankUtilityApiResponse();

            try
            {

                System.Net.Http.HttpResponseMessage apiResponse = await _apiConfig.ApiUrl
                                    .AppendPathSegment("/devices")                                    
                                    .WithOAuthBearerToken(await GetBearerAuthHeaderValue())
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

            return response;
        }

        public async Task<TankUtilityApiResponse> GetDeviceAsync(string tankId)
        {
            List<string> jsonErrors = new List<string>();

            TankUtilityApiResponse response = new TankUtilityApiResponse();

            try
            {
                var accessToken = await GetBearerAuthHeaderValue();


                System.Net.Http.HttpResponseMessage apiResponse = await _apiConfig.ApiUrl
                                    .AppendPathSegment("devices")
                                    .AppendPathSegment($"{tankId}")                                   
                                    .WithOAuthBearerToken(accessToken)                                    
                                    .GetAsync();

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
                // any exception not related to the API web request e.g. JSON serialization errors, null args etc.
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

            try
            {

                System.Net.Http.HttpResponseMessage apiResponse = await _apiConfig.ApiUrl
                                    .AppendPathSegment("/readings/")
                                    .AppendPathSegment($"{tankId}")
                                    .WithOAuthBearerToken(await GetBearerAuthHeaderValue())
                                    .GetAsync();

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
                response.ErrorTitle = $"Get Device Readings Request Timeout For - Tank Id {tankId} - Exception.";
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
                response.ErrorTitle = $"Get Device Readings Request For - Tank Id {tankId} - Exception.";
            }
            catch (Exception anException)
            {
                // any exception not related to the API web request e.g. JSON serialization errors, null args etc.
                _logger.Error("Tank Utility Exception.", anException);

                response.Success = false;
                response.JsonParsingErrors = jsonErrors;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorTitle = $"Get Device Readings Request For - Tank Id {tankId} - Exception.";
            }

            return response;
        }

        public async Task<TankUtilityApiResponse> PatchDeviceConfigAsync(string tankId, Dictionary<string, object> tankConfiguration)
        {
            List<string> jsonErrors = new List<string>();

            TankUtilityApiResponse response = new TankUtilityApiResponse();

            try
            {

                System.Net.Http.HttpResponseMessage apiResponse = await _apiConfig.ApiUrl
                                    .AppendPathSegment("/devices/")
                                    .AppendPathSegment($"{tankId}")
                                    .WithOAuthBearerToken(await GetBearerAuthHeaderValue())
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
                response.ErrorTitle = $"Patch Device Configuration Request Timeout For - Tank Id {tankId} - Exception.";
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
                response.ErrorTitle = $"Patch Device Configuration Request For - Tank Id {tankId} - Exception.";
            }
            catch (Exception anException)
            {
                // any exception not related to the API web request e.g. JSON serialization errors, null args etc.
                _logger.Error("Tank Utility Exception.", anException);

                response.Success = false;
                response.JsonParsingErrors = jsonErrors;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorTitle = $"Patch Device Configuration Request For - Tank Id {tankId} - Exception.";
            }

            return response;
        }
        #endregion Public Methods

        #region Private Methods

        private static async Task<OauthTokenResponse> GetOathTokenResponse()
        {
            OauthTokenResponse response;            

            var encodedSecret = HttpUtility.UrlEncode(_apiConfig.AuthClientSecret);
            var encodedRefreshToken = HttpUtility.UrlEncode(_apiConfig.AuthRefreshToken);
            //var postBody = $"grant_type=client_credentials&client_id={_apiConfig.AuthClientId}&client_secret={encodedSecret}";
            var postBody = $"grant_type=refresh_token&client_id={_apiConfig.AuthClientId}&client_secret={encodedSecret}&refresh_token={encodedRefreshToken}";                

            try
            {
                System.Net.Http.HttpResponseMessage apiResponse = await _apiConfig.AuthUrl
                                .PostUrlEncodedAsync(postBody);                

                var apiResponseStr = await apiResponse.Content?.ReadAsStringAsync();


                response = JsonConvert.DeserializeObject<OauthTokenResponse>(apiResponseStr, _jsonSerializerSettings);
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
