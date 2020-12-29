using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TankNotifierApiNet.Responses;
using System.Web.Http;


namespace TankNotifierApiNet.Controllers
{
    using Common.Logging;
    using RedisCacheLib;
    using TankUtilityApiLib.TankUtilityApi;
    using TankUtilityApiLib.Models;
    using TankNotifierApiNet.Models;
    using System.Net.Http;
    using System.Text;
    using Newtonsoft.Json;
    using System.Net;
    using System.Linq;

    [Authorize]
    [RoutePrefix("api/skybitz")]
    public class SkyBitzApiController : ApiController
    {
        #region
        private static readonly ILog _logger = LogManager.GetLogger("TankLinkNotifier");
        private static readonly RedisCacheManager _redisCacheManager = RedisCacheManager.Instance;
        private static readonly ServiceBusTopicSender _serviceBusTopicSender = ServiceBusTopicSender.Instance;
        private static readonly TankUtilityApi _tankUtilityApi = TankUtilityApi.Instance;

        private const string CACHE_KEY_PREFIX = "tankLinkNotifier_";
        
        // No longer used.  This key points to the concatenated common separated string value ("short TU key, long TU key")
        //private const string _cacheKeyPrefix_OBSOLETE = "tankUtility_cached_Id_";

        #endregion

        public SkyBitzApiController()
        {

        }
       
        [HttpPost]
        [Route("tank/{tankId}/reading")]        
        public async Task<ApiResponse> PostReading(string tankId, [FromBody] Dictionary<string, object> tankReading)
        {
            var apiResponse = new ApiResponse();

            var paramErrors = new Dictionary<string, object>();            

            try
            {                

                if (string.IsNullOrWhiteSpace(tankId))
                {
                    paramErrors.Add("tankId", "Is null, empty, or whitspace.");
                }

                var validationResponse = await ValidateTankId(tankId);

                if (!validationResponse.Success)
                {
                    paramErrors.Add("tankId_validation", $"Failed:  {validationResponse.ErrorMessage}");
                }

                if ((tankReading == null) || !tankReading.Any())
                {
                    paramErrors.Add("tankReading", "Is null or empty.");
                }

                if (paramErrors.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.Message = $"Error receiving tank reading for Tank Id:  {tankId}";
                    apiResponse.ErrorMessage = $"POST Tank Reading:  Missing Parameters";
                    apiResponse.ParameterErrors = paramErrors;

                    return SendResponse(apiResponse, HttpStatusCode.BadRequest);
                }                

                var payload = new Payload
                {
                    TankId = validationResponse.ShortTankId,
                    LongTankId = validationResponse.LongTankId,
                    Description = $"Tank Utility Reading:  {validationResponse.ShortTankId} ({validationResponse.LongTankId}).",
                    Data = tankReading,
                    ReceivedOn = DateTime.UtcNow
                };

                var customProperties = new Dictionary<string, object>
                {
                    { "EventType" , "UPDATE_TU_TANK_READING"},
                    { "TankId", validationResponse.ShortTankId },
                    { "LongTankId", validationResponse.LongTankId }
                };

                await _serviceBusTopicSender.SendMessage(payload, customProperties);

                apiResponse.Success = true;
                apiResponse.Message = $"Tank reading received for:  {validationResponse.ShortTankId} at {payload.ReceivedOn:O}.";
            }
            catch (HttpResponseException)
            {
                // The SendResponse method will generate an exception (Bad Request etc.) so just re-throw it to return it to the user.              
                throw;
            }
            catch (Exception anException)
            {
                var tankReadingStr = SerializeDictionary(tankReading);

                _logger.Error($"POST Tank Reading:  {anException.Message} {anException?.InnerException?.Message}");                

                _logger.Error($"Tank Reading:  Id:  {tankId}  Reading:  {tankReadingStr}");

                apiResponse.Success = false;
                apiResponse.Message = $"Error receiving tank reading for Tank Id:  {tankId}";
                apiResponse.ErrorMessage = $"POST Tank Reading:  {anException.Message} {anException?.InnerException?.Message}";

                return SendResponse(apiResponse, HttpStatusCode.InternalServerError);
            }

            return SendResponse(apiResponse, HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("tank/{tankId}")]
        public async Task<ApiResponse> PostConfigChange(string tankId, [FromBody] Dictionary<string, object> tankConfig)
        {
            var apiResponse = new ApiResponse();

            var paramErrors = new Dictionary<string, object>();            

            try
            {                

                if (string.IsNullOrWhiteSpace(tankId))
                {
                    paramErrors.Add("tankId", "Is null, empty, or whitspace.");
                }                

                var validationResponse = await ValidateTankId(tankId);

                if (!validationResponse.Success)
                {
                    paramErrors.Add("tankId_validation", $"Failed:  {validationResponse.ErrorMessage}");
                }

                if ((tankConfig == null) || !tankConfig.Any())
                {
                    paramErrors.Add("tankConfig", "Is null or empty.");
                }
                

                if (paramErrors.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.Message = $"Error receiving configuration update for Tank Id:  {tankId}";
                    apiResponse.ErrorMessage = $"POST Tank Configuration:  Missing Parameters";
                    apiResponse.ParameterErrors = paramErrors;

                    return SendResponse(apiResponse, HttpStatusCode.BadRequest);
                }                

                var payload = new Payload
                {
                    TankId = validationResponse.ShortTankId,
                    LongTankId = validationResponse.LongTankId,
                    Description = $"Tank Utility Configuration Change:  {validationResponse.ShortTankId}.",
                    Data = tankConfig,
                    ReceivedOn = DateTime.UtcNow
                };

                var customProperties = new Dictionary<string, object>
                {
                    { "EventType" , "UPDATE_TU_TANK_CONFIG"},
                    { "TankId", validationResponse.ShortTankId },
                    { "LongTankId", validationResponse.LongTankId }
                };

                await _serviceBusTopicSender.SendMessage(payload, customProperties);

                apiResponse.Success = true;
                apiResponse.Message = $"Configuration update received for:  {validationResponse.ShortTankId} at {payload.ReceivedOn:O}.";
            }
            catch (HttpResponseException)
            {
                // The SendResponse method will generate an exception (Bad Request etc.) so just re-throw it to return it to the user.              
                throw;
            }
            catch (Exception anException)
            {
                var tankConfigStr = SerializeDictionary(tankConfig);

                _logger.Error($"POST Tank Config Change:  {anException.Message} {anException?.InnerException?.Message}");

                _logger.Error($"Tank Config Change:  Id:  {tankId}  Reading:  {tankConfigStr}");

                apiResponse.Success = false;
                apiResponse.Message = $"Error receiving tank config change for Tank Id:  {tankId}";
                apiResponse.ErrorMessage = $"POST Tank Config Change:  {anException.Message} {anException?.InnerException?.Message}";

                return SendResponse(apiResponse, HttpStatusCode.InternalServerError);
            }

            return SendResponse(apiResponse, HttpStatusCode.OK);
        }


        private async Task<TankIdValidationResponse> ValidateTankId(string tankId)
        {
            TankIdValidationResponse validationResponse = new TankIdValidationResponse();
            TankUtilityCacheDevice cacheEntry;


            try
            {
                // Is there a cache entry for the given key.
                var tankIdCacheEntry = _redisCacheManager.GetFromCache<TankUtilityCacheDevice>($"{CACHE_KEY_PREFIX}{tankId?.ToUpper()}");


                // if it's not in cache check Tank Utility via API or the cache entry is malformed.
                if (tankIdCacheEntry == null)
                {
                    // Get the device record from Tank Utility
                    var apiResponse = await _tankUtilityApi.GetDeviceAsync(tankId?.ToUpper());

                    // Mark the validation response.
                    validationResponse.Success = apiResponse.Success;
                    validationResponse.ErrorMessage = apiResponse.ErrorMessage;

                    cacheEntry = new TankUtilityCacheDevice
                    {
                        ShortTankId = tankId,
                        LongTankId = tankId,
                        IsValidTankId = false,
                        ErrorMessage = validationResponse.ErrorMessage
                    };


                    // if successfully retrieved from Tank Utility add both long and short id's to the cache.
                    if (validationResponse.Success)
                    {
                        // Make sure we have data.
                        if (apiResponse.Data as DeviceResponse != null)
                        {
                            // Set the short and long id in the validation response.
                            validationResponse.ShortTankId = (apiResponse.Data as DeviceResponse).Device.ShortDeviceId.ToUpper();
                            validationResponse.LongTankId = (apiResponse.Data as DeviceResponse).Device.DeviceId.ToUpper();

                            // Update the cache entry object with the "good" tank Id values.
                            cacheEntry.ShortTankId = validationResponse.ShortTankId;
                            cacheEntry.LongTankId = validationResponse.LongTankId;
                            cacheEntry.IsValidTankId = validationResponse.Success;
                        }
                        else
                        {
                            cacheEntry.ErrorMessage = $"Tank Id:  {cacheEntry.ShortTankId}  Tank Utility API call indicated \"Success\" but did not return a valid \"Device Response\" object.";
                        }
                    }

                    // Store the cache key and cache entry data.                    
                    // If it's a valid tank Id entry then store the cache entry by the long id too in case we need to search by that id.
                    if (cacheEntry.IsValidTankId)
                    {
                        _redisCacheManager.AddToCache($"{CACHE_KEY_PREFIX}{cacheEntry.ShortTankId.ToUpper()}", cacheEntry);
                        _redisCacheManager.AddToCache($"{CACHE_KEY_PREFIX}{cacheEntry.LongTankId.ToUpper()}", cacheEntry);
                    }                    
                }
                else
                {
                    // Get info from the cache entry.                      
                    // Set the short and long id, and the "success" (IsValid) flag in the validation response.
                    validationResponse.ShortTankId = tankIdCacheEntry.ShortTankId;
                    validationResponse.LongTankId = tankIdCacheEntry.LongTankId;
                    validationResponse.Success = tankIdCacheEntry.IsValidTankId;
                    validationResponse.ErrorMessage = tankIdCacheEntry.ErrorMessage;
                }

            }
            catch (Exception anException)
            {
                _logger.Error($"Tank Id Validation:  Tank Id:  {tankId} - {anException.Message} {anException?.InnerException?.Message}");

                validationResponse.ShortTankId = tankId;
                validationResponse.LongTankId = tankId;
                validationResponse.Success = false;
                validationResponse.ErrorMessage = $"Tank Id Validation:  Tank Id:  {tankId} -  {anException.Message} {anException?.InnerException?.Message}";
            }

            return validationResponse;
        }

        private T SendResponse<T>(T response, System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.OK)
        {
            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                // leave it up to microsoft to make this way more complicated than it needs to be
                // seriously i used to be able to just set the status and leave it at that but nooo... now 
                // i need to throw an exception 
                var badResponse =
                    new HttpResponseMessage(statusCode)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json")
                    };

                throw new HttpResponseException(badResponse);               
            }
            return response;
        }

        private string SerializeDictionary( Dictionary<string, object> aDictionary)
        {
            string returnValue;

            try
            {
                if (aDictionary != null)
                {
                    returnValue = JsonConvert.SerializeObject(aDictionary);
                }
                else
                {
                    returnValue = "{}";
                }
                
            }
            catch
            {
                returnValue = "{}";
            }

            return returnValue;
        }
    }
}