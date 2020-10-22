using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TankLinkNotifierApi.TankUtilityApi
{
    using Flurl;
    using Flurl.Http;
    using Flurl.Http.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System.Web;
    using TankLinkNotifierApi.Models;
    using System.Net;
    using Newtonsoft.Json;
    using Microsoft.Extensions.Caching.Memory;

    public class TankUtilityApi
    {
        #region Private fields        

        private static readonly IFlurlClientFactory _fluentClientFactory = new PerBaseUrlFlurlClientFactory();

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore };

        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private const string _cacheKeyPrefix = "tankUtility_api_";

        private static ApiOauthConfiguration _apiConfig;
        
        #endregion Private fields


        #region Constructors

        public TankUtilityApi(IConfiguration configuration, IMemoryCache memoryCache, ILogger<TankUtilityApi> logger)
        {
            try
            {
                _configuration = configuration;
                _logger = logger;
                _memoryCache = memoryCache;

                _apiConfig = new ApiOauthConfiguration();

                _configuration.Bind("TankUtilityConfiguration", _apiConfig);

                if (_apiConfig == null)
                {
                    throw new NullReferenceException("No api configuration (Section: TankUtilityConfiguration) was retrieved from app config file.");
                }

                _fluentClientFactory.Get(_apiConfig.AuthUrl);

                _fluentClientFactory.Get(_apiConfig.ApiUrl);

            }
            catch (Exception anException)
            {
                _logger.LogError($"Tank Utility API Constructor Error:  {anException.Message} {anException?.InnerException?.Message}");
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
                _logger.LogError($"Tank Utility API Constructor Error:  {anException.Message} {anException?.InnerException?.Message}");
                _apiConfig = new ApiOauthConfiguration();
            }
        }

        #endregion Constructors     

        #region Public Methods        

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
                _logger.LogError("Tank Utility Request Timed Out.", anException);

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
                _logger.LogError("Tank Utility Request Exception.", anException);

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
                _logger.LogError("Tank Utility Exception.", anException);

                response.Success = false;
                response.JsonParsingErrors = jsonErrors;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorTitle = $"Get Device Request For - Tank Id {tankId} - Exception.";
            }

            return response;
        }        

        #endregion Public Methods

        #region Private Methods


        private async Task<OauthTokenResponse> GetOathTokenResponse()
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

        public async Task<string> GetBearerAuthHeaderValue()
        {
            //try to get token from cache

            var bearerTokenKey = $"{_cacheKeyPrefix}bearer_token";

            if (!_memoryCache.TryGetValue(bearerTokenKey,  out OauthTokenResponse tokenResponse))
            {
                // nothing in cache, get a new token
                tokenResponse = await GetOathTokenResponse();

                if (tokenResponse != null)
                {
                    _memoryCache.Set(bearerTokenKey, tokenResponse);
                }

            }
            else
            {
                // got something is it expired or about to be expired?
                if (tokenResponse.ExpiresOn <= DateTime.UtcNow.AddSeconds(60))
                {

                    tokenResponse = await GetOathTokenResponse();
                    _memoryCache.Set(bearerTokenKey, tokenResponse);
                }
            }
            return $"{(tokenResponse != null ? tokenResponse.access_token : string.Empty)}";
        }
        #endregion Private Methods
    }


}
