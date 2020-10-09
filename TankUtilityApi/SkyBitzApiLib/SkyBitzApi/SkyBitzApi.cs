using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkyBitzApiLib.SkyBitzApi
{
    using Flurl;
    using Flurl.Http;
    using Flurl.Http.Configuration;
    using Common.Logging;
    using System.Configuration;
    using System.Web;
    using System.Runtime.Caching;
    using SkyBitzApiLib.Models;
    using System.Net;
    using Newtonsoft.Json;
    using System.Net.Mime;
    using Flurl.Util;

    public class SkyBitzApi
    {
        #region Private fields

        private static readonly ILog _logger = LogManager.GetLogger("SkyBitzApi");

        private static readonly IFlurlClientFactory _fluentClientFactoryBaseUrl = new PerBaseUrlFlurlClientFactory();        

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore };

        private static ApiOauthConfiguration _apiConfig;

        #endregion Private fields

        #region Constructors

        public SkyBitzApi()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                _apiConfig = ConfigurationManager.GetSection("SkyBitzConfiguration") as ApiOauthConfiguration;

                if (_apiConfig == null)
                {
                    throw new NullReferenceException("No api configuration (Section: SkyBitzConfiguration) was retrieved from app config file.");
                }

                _fluentClientFactoryBaseUrl.Get(_apiConfig.AuthUrl);

                _fluentClientFactoryBaseUrl.Get(_apiConfig.ApiUrl);                

            }
            catch (Exception anException)
            {
                _logger.Error($"SkyBitz API Constructor Error:  {anException.Message} {anException?.InnerException?.Message}");
                _apiConfig = new ApiOauthConfiguration();
            }
        }

        public SkyBitzApi(ApiOauthConfiguration apiConfig)
        {
            try
            {
                _apiConfig = apiConfig;

                if (_apiConfig == null)
                {
                    throw new NullReferenceException("No api configuration (Section: SkyBitzConfiguration) was retrieved from app config file.");
                }

                _fluentClientFactoryBaseUrl.Get(_apiConfig.AuthUrl);

                _fluentClientFactoryBaseUrl.Get(_apiConfig.ApiUrl);                

            }
            catch (Exception anException)
            {
                _logger.Error($"SkyBitz API Constructor Error:  {anException.Message} {anException?.InnerException?.Message}");
                _apiConfig = new ApiOauthConfiguration();
            }
        }

        #endregion Constructors

        #region Public Methods

        public async Task<SkyBitzApiResponse> PostDeviceConfigChangeAsync(string tankId, Dictionary<string, object> tankConfiguration)
        {
            List<string> jsonErrors = new List<string>();

            SkyBitzApiResponse response = new SkyBitzApiResponse();

            try
            {
                IFlurlClient aClient = _fluentClientFactoryBaseUrl.Get(_apiConfig.ApiUrl);


                System.Net.Http.HttpResponseMessage apiResponse = await aClient.Request($"{tankId}")
                                                                        .WithOAuthBearerToken(await GetBearerAuthHeaderValue())
                                                                        .PostJsonAsync(tankConfiguration);

                //System.Net.Http.HttpResponseMessage apiResponse = await _apiConfig.ApiUrl
                //                    .AppendPathSegment($"{tankId}")
                //                    .WithOAuthBearerToken(await GetBearerAuthHeaderValue())
                //                    .PostJsonAsync(tankConfiguration);


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
                _logger.Error("SkyBitz API Request Timed Out.", anException);

                response.Success = false;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorCode = (int)anException.Call?.HttpStatus;
                response.HttpStatusCode = (int)anException.Call?.HttpStatus;
                response.RequestProperties = new Dictionary<string, object>(anException?.Call?.Request.Properties);
                response.ErrorTitle = $"Post Device Configuration Change Request Timeout For - Tank Id {tankId} - Exception.";
            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)                
                _logger.Error("SkyBitz API Request Exception.", anException);

                response.Success = false;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorCode = (int)anException.Call?.HttpStatus;
                response.HttpStatusCode = (int)anException.Call?.HttpStatus;
                response.RequestProperties = new Dictionary<string, object>(anException.Call?.Request.Properties);
                response.ErrorTitle = $"Post Device Configuration Change Request For - Tank Id {tankId} - Exception.";
            }
            catch (Exception anException)
            {
                // any exception not related to the API web request e.g. JSON serialization errors, null args etc.
                _logger.Error("SkyBitz API Exception.", anException);

                response.Success = false;
                response.JsonParsingErrors = jsonErrors;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorTitle = $"Post Device Configuration Change Request For - Tank Id {tankId} - Exception.";
            }

            return response;
        }

        public async Task<SkyBitzApiResponse> PostTankReadingAsync(string tankId, Dictionary<string, object> tankReading)
        {
            List<string> jsonErrors = new List<string>();

            SkyBitzApiResponse response = new SkyBitzApiResponse();

            try
            {
                IFlurlClient aClient = _fluentClientFactoryBaseUrl.Get(_apiConfig.ApiUrl);

                System.Net.Http.HttpResponseMessage apiResponse = await aClient.Request($"{tankId}", "reading")                     
                                                                        .WithOAuthBearerToken(await GetBearerAuthHeaderValue())
                                                                        .PostJsonAsync(tankReading);

                //System.Net.Http.HttpResponseMessage apiResponse = await _apiConfig.ApiUrl
                //                    .AppendPathSegment($"{tankId}")
                //                    .AppendPathSegment("reading")
                //                    .WithOAuthBearerToken(await GetBearerAuthHeaderValue())
                //                    .PostJsonAsync(tankReading);


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
                _logger.Error("SkyBitz API Request Timed Out.", anException);

                response.Success = false;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorCode = (int)anException.Call?.HttpStatus;
                response.HttpStatusCode = (int)anException.Call?.HttpStatus;
                response.RequestProperties = new Dictionary<string, object>(anException?.Call?.Request.Properties);
                response.ErrorTitle = $"Post Tank Reading Request Timeout For - Tank Id {tankId} - Exception.";
            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)                
                _logger.Error("SkyBitz API Request Exception.", anException);

                response.Success = false;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorCode = (int)anException.Call?.HttpStatus;
                response.HttpStatusCode = (int)anException.Call?.HttpStatus;
                response.RequestProperties = new Dictionary<string, object>(anException.Call?.Request.Properties);
                response.ErrorTitle = $"Post Tank Reading Request For - Tank Id {tankId} - Exception.";
            }
            catch (Exception anException)
            {
                // any exception not related to the API web request e.g. JSON serialization errors, null args etc.
                _logger.Error("SkyBitz API Exception.", anException);

                response.Success = false;
                response.JsonParsingErrors = jsonErrors;
                response.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                response.ErrorTitle = $"Post Tank Reading Request For - Tank Id {tankId} - Exception.";
            }

            return response;
        }

        #endregion Public Methods

        #region Private Methods

        private static async Task<OauthTokenResponse> GetOathTokenResponse()
        {
            OauthTokenResponse response;

            var encodedSecret = HttpUtility.UrlEncode(_apiConfig.AuthClientSecret);
            
            var postBody = $"grant_type=client_credentials&client_id={_apiConfig.AuthClientId}&client_secret={encodedSecret}&scope={_apiConfig.AuthScopeId}";
            

            try
            {
                System.Net.Http.HttpResponseMessage apiResponse = await _apiConfig.AuthUrl
                                .AppendPathSegment($"{_apiConfig.AuthTenantId}")
                                .AppendPathSegment("/oauth2/v2.0/token")
                                .PostUrlEncodedAsync(postBody);

                var apiResponseStr = await apiResponse.Content?.ReadAsStringAsync();


                response = JsonConvert.DeserializeObject<OauthTokenResponse>(apiResponseStr, _jsonSerializerSettings);
            }
            catch (FlurlHttpTimeoutException anException)
            {
                // FlurlHttpTimeoutException derives from FlurlHttpException; catch here only
                // if you want to handle timeouts as a special case                
                throw new SkyBitzApiException((int)anException.Call.HttpStatus, anException.Message, anException);
            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)
                throw new SkyBitzApiException((int)anException.Call.HttpStatus, anException.Message, anException);
            }
            catch (Exception anException)
            {
                // Garden variety exception.  Flag it as 500 server error.
                throw new SkyBitzApiException(500, anException.Message, anException);
            }

            return response;
        }

        public static async Task<string> GetBearerAuthHeaderValue()
        {
            //try to get token from cache

            if (!(MemoryCache.Default["skybitz_bearer_token"] is OauthTokenResponse tokenResponse))
            {
                // nothing in cache, get a new token
                tokenResponse = await GetOathTokenResponse();

                if (tokenResponse != null)
                {
                    MemoryCache.Default["skybitz_bearer_token"] = tokenResponse;
                }

            }
            else
            {
                // got something is it expired or about to be expired?
                if (tokenResponse.ExpiresOn <= DateTime.UtcNow.AddSeconds(60))
                {

                    tokenResponse = await GetOathTokenResponse();
                    MemoryCache.Default["skybitz_bearer_token"] = tokenResponse;
                }
            }
            return $"{(tokenResponse != null ? tokenResponse.access_token : string.Empty)}";
        }
        #endregion Private Methods
    }
}
