using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using TankLinkNotifierApi.Models;
using TankLinkNotifierApi.Responses;
using Microsoft.Extensions.Caching.Distributed;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TankLinkNotifierApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/skybitz")]
    public class SkyBitzApiController : ControllerBase
    {
        private readonly ServiceBusTopicSender _serviceBusTopicSender;
        private readonly ILogger<SkyBitzApiController> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly TankUtilityApi.TankUtilityApi _tankUtilityApi;
        private const string _cacheKeyPrefix = "tankUtility_Id_";

        public SkyBitzApiController(ServiceBusTopicSender serviceBusTopicSender, TankUtilityApi.TankUtilityApi tankUtilityApi, IDistributedCache distributedCache, ILogger<SkyBitzApiController> logger)
        {
            _serviceBusTopicSender = serviceBusTopicSender;
            _distributedCache = distributedCache;
            _tankUtilityApi = tankUtilityApi;
            _logger = logger;
        }

        [HttpPost("tank/{tankId}/reading")]
        public async Task<ActionResult<ApiResponse>> PostReading(string tankId, [FromBody] Dictionary<string, object> tankReading)
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

                    return BadRequest(apiResponse);
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
            catch(Exception anException)
            {
                _logger.LogError($"POST Tank Reading:  {anException.Message} {anException?.InnerException?.Message}");
                _logger.LogError($"Tank Reading:  {tankId}", tankReading);

                apiResponse.Success = false;
                apiResponse.Message = $"Error receiving tank reading for Tank Id:  {tankId}";
                apiResponse.ErrorMessage = $"POST Tank Reading:  {anException.Message} {anException?.InnerException?.Message}";

                return StatusCode(500, apiResponse);
            }

            return Ok(apiResponse);
        }
      
        [HttpPost("tank/{tankId}/")]
        public async Task<ActionResult<ApiResponse>> PostConfig(string tankId, [FromBody] Dictionary<string, object> tankConfig)
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

                    return BadRequest(apiResponse);
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
            catch (Exception anException)
            {
                _logger.LogError($"POST Tank Config:  {anException.Message} {anException?.InnerException?.Message}");
                _logger.LogError($"Tank Config:  {tankId}", tankConfig);

                apiResponse.Success = false;
                apiResponse.Message = $"Error receiving configuration update for Tank Id:  {tankId}";
                apiResponse.ErrorMessage = $"POST Tank Configuration:  {anException.Message} {anException?.InnerException?.Message}";

                return StatusCode(500, apiResponse);
            }

            return Ok(apiResponse);
        }   
        
        private async Task<TankIdValidationResponse> ValidateTankId(string tankId)
        {
            TankIdValidationResponse validationResponse = new TankIdValidationResponse();
            

            try
            {
                // Is there a cache entry for the given key.
                var tankIdCacheEntry = await _distributedCache.GetStringAsync($"{_cacheKeyPrefix}{tankId?.ToUpper()}");

                // Cach entry is a comma separated string pair formatted as:  "{tank short id},{tank long id}"
                var tankIds = tankIdCacheEntry?.Split(',');
               

                // if it's not in cache check Tank Utility via API or the cache entry is malformed.
                if (string.IsNullOrWhiteSpace(tankIdCacheEntry) || ((tankIds?.Length ?? 0) != 2))
                {
                    // Get the device record from Tank Utility
                    var apiResponse = await _tankUtilityApi.GetDeviceAsync(tankId?.ToUpper());

                    // Mark the validation response.
                    validationResponse.Success = apiResponse.Success;
                    validationResponse.ErrorMessage = apiResponse.ErrorMessage;

                    // if successfully retrieved from Tank Utility add both long and short id's to the cache.
                    if (validationResponse.Success)
                    {
                        // Make sure we have data.
                        if (apiResponse.Data as DeviceResponse != null)
                        {
                            // Set the short and long id in the validation response.
                            validationResponse.ShortTankId = (apiResponse.Data as DeviceResponse).Device.ShortDeviceId.ToUpper();
                            validationResponse.LongTankId = (apiResponse.Data as DeviceResponse).Device.DeviceId;


                            var cachEntryOptions = new DistributedCacheEntryOptions();

                            cachEntryOptions.SetSlidingExpiration(TimeSpan.FromDays(5));
                            cachEntryOptions.SetAbsoluteExpiration(TimeSpan.FromDays(30));

                            // Store the cache key and data (data is a comma separated string pair formatted as:  "{tank short id},{tank long id}").
                            await _distributedCache.SetStringAsync($"{_cacheKeyPrefix}{validationResponse.ShortTankId}", $"{validationResponse.ShortTankId},{validationResponse.LongTankId}", cachEntryOptions);
                            await _distributedCache.SetStringAsync($"{_cacheKeyPrefix}{validationResponse.LongTankId}", $"{validationResponse.ShortTankId},{validationResponse.LongTankId}", cachEntryOptions);
                        }                        
                    }
                }
                else
                {
                    // Split the cache entry.  Cach entry is a comma separated string pair formatted as:  "{tank short id},{tank long id}"
                    tankIds = tankIdCacheEntry.Split(',');
                    // Make sure it has two strings.
                    validationResponse.Success = (tankIds?.Length ?? 0) == 2;
                    // Set the short and long id in the validation response.
                    validationResponse.ShortTankId = tankIds[0];
                    validationResponse.LongTankId = tankIds[1];
                }
                                
            }
            catch (Exception anException)
            {
                _logger.LogError($"Tank Id Validation:  Tank Id:  {tankId} - {anException.Message} {anException?.InnerException?.Message}");
                

                validationResponse.Success = false;                
                validationResponse.ErrorMessage = $"Tank Id Validation:  Tank Id:  {tankId} -  {anException.Message} {anException?.InnerException?.Message}";                
            }

            return validationResponse;
        }
    }

}
