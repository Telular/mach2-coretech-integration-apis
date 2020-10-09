using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Linq;

namespace AssetLinkGlobalApiLib
{
    using Common.Logging;
    using Flurl;
    using Flurl.Http;
    using Flurl.Http.Configuration;

    using AssetLinkGlobalApiLib.Group;
    using AssetLinkGlobalApiLib.Device;
    using AssetLinkGlobalApiLib.Moment;

    using AssetLinkGlobalApiLib.Group.Models;
    using AssetLinkGlobalApiLib.Device.Models;
    using AssetLinkGlobalApiLib.Moment.Models;

    using Newtonsoft.Json;

    public class AssetLinkGlobalApi
    {
        private static readonly ILog _logger = LogManager.GetLogger("AssetLinkApiUsage");
        private static ApiConfiguration _apiConfig;
        private static readonly IFlurlClientFactory _fluentClientFactory = new PerBaseUrlFlurlClientFactory();
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore };
        
        public AssetLinkGlobalApi()
        {
            try
            {
                _apiConfig = ConfigurationManager.GetSection("AssetLinkSettings") as ApiConfiguration;

                var baseUrlClient = _fluentClientFactory.Get(_apiConfig.BaseUrl)
                                .EnableCookies()
                                .WithHeader("Content-Type", "application/json");
            }
            catch (Exception anException)
            {
                _logger.Error("Failed to initialize AssetLinkGlobalApi", anException);
                _apiConfig = new ApiConfiguration();
            }

        }

        public AssetLinkGlobalApi(ApiConfiguration apiConfig)
        {
            try
            {
                _apiConfig = apiConfig ?? throw new Exception("Configuration file settings for Asset Link Global are Undefined.");

                var baseUrlClient = _fluentClientFactory.Get(apiConfig.BaseUrl)
                                .EnableCookies()
                                .WithHeader("Content-Type", "application/json");
            }
            catch (Exception anException)
            {
                _logger.Error("Failed to initialize AssetLinkGlobalApi", anException);
                _apiConfig = new ApiConfiguration();
            }

        }

        public ApiConfiguration ConfiurationSettings => _apiConfig;

        public async Task<LoginResponse> Login()
        {

            LoginResponse returnValue = new LoginResponse();

            try
            {

                var login = new Login
                {
                    User = new User
                    {
                        Name = _apiConfig.Username,
                        Password = _apiConfig.Password
                    }
                };

                returnValue = await _apiConfig.BaseUrl
                                    .AppendPathSegment(_apiConfig.AuthUrlPath)
                                    .PostJsonAsync(login)
                                    .ReceiveJson<LoginResponse>();

                returnValue.Success = returnValue.User.Status == "VERIFIED";
                
            }
            catch (FlurlHttpTimeoutException anException)
            {

                // FlurlHttpTimeoutException derives from FlurlHttpException; catch here only
                // if you want to handle timeouts as a special case
                _logger.Error("Asset Link Global API Login Failure. Timeout Error.", anException);

                returnValue.User.Name = _apiConfig.Username;
                returnValue.User.Status = "UNVERIFIED";
                returnValue.Success = false;
                returnValue.ErrorCode = (int)anException.Call?.HttpStatus;
                returnValue.ErrorMessage = $"{anException.Message}  {(anException.InnerException?.Message)}";
                returnValue.ErrorTitle = "Asset Link Global API Login Failure.";
            }
            catch (FlurlHttpException anException)
            {
                // FlurlHttpException contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)
                _logger.Error("Asset Link Global API Login Failure.", anException);

                returnValue.User.Name = _apiConfig.Username;
                returnValue.User.Status = "UNVERIFIED";
                returnValue.Success = false;
                returnValue.ErrorCode = (int)anException.Call?.HttpStatus;
                returnValue.ErrorMessage = $"{anException.Message}  {(anException.InnerException?.Message)}";
                returnValue.ErrorTitle = "Asset Link Global API Login Failure.";
            }
            catch (Exception anException)
            {
                _logger.Error("Asset Link Global API Login Failure.", anException);

                returnValue.User.Name = _apiConfig.Username;
                returnValue.User.Status = "UNVERIFIED";
                returnValue.Success = false;
                returnValue.ErrorMessage = $"{anException.Message}  {(anException.InnerException?.Message)}";
                returnValue.ErrorTitle = "Asset Link Global API Login Failure.";
            }

            return returnValue;
        }

        public async Task<bool> Logout()
        {
            bool returnValue = false;


            try
            {
                var logoutResponse = await _apiConfig.BaseUrl
                                            .AppendPathSegment(_apiConfig.AuthUrlPath)
                                            .PostJsonAsync(new Logout());

                returnValue = logoutResponse.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (FlurlHttpTimeoutException anException)
            {

                // FlurlHttpTimeoutException derives from FlurlHttpException; catch here only
                // if you want to handle timeouts as a special case
                _logger.Error("Logout Request timed out.", anException);
            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)
                _logger.Error($"Logout Failure:  {anException.Message}", anException);
            }
            catch (Exception anException)
            {
                _logger.Error("Asset Link API Client Logout Failure", anException);
            }


            return returnValue;
        }

        public async Task<DeviceApiResponse> ProcessDevicesRequest(ApiRequest apiRequest)
        {
            var returnValue = new DeviceApiResponse();

            try
            {
                var response = await ProcessApiRequest(new List<ApiRequest> { apiRequest });

                var apiResponseStr = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    try
                    {
                        // Get the device data list if there is actually data.
                        var dataList = JsonConvert.DeserializeObject<List<DeviceData>>(apiResponseStr, _jsonSerializerSettings);

                        var deviceData = dataList?.FirstOrDefault();

                        returnValue.Data = deviceData?.Data?.Devices;
                        returnValue.Result = deviceData?.Result;
                    }
                    catch
                    {
                        returnValue.ApiResponseStr = apiResponseStr ?? "No Response Content Returned.";

                        // See if this is an empty object (i.e. Asset Link returned empty arrays).
                        // If it's not an empty object list then this will exception out.
                        var dataList = JsonConvert.DeserializeObject<List<EmptyObject>>(apiResponseStr, _jsonSerializerSettings);

                        var deviceData = dataList?.FirstOrDefault();

                        returnValue.Data = new List<Device.Models.Device>();
                        returnValue.Result = deviceData?.Result;
                    }


                    returnValue.HttpStatusCode = (int)response.StatusCode;

                    returnValue.Success = true;
                }
                else
                {
                    returnValue.ApiResponseStr = apiResponseStr ?? "No Response Content Returned.";

                    returnValue.Result = apiResponseStr;

                    returnValue.HttpStatusCode = (int)response.StatusCode;

                    returnValue.Success = false;
                }

            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)
                _logger.Error($"Process Devices Request Failure", anException);

                returnValue.Success = false;
                returnValue.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                returnValue.ErrorCode = (int)anException.Call?.HttpStatus;
                returnValue.RequestProperties = new Dictionary<string, object>(anException.Call?.Request.Properties);
                returnValue.ErrorTitle = "Process Devices Request Failure.";


            }
            catch (Exception anException)
            {
                _logger.Error($"Process Devices Request Failure.", anException);

                returnValue.Success = false;
                returnValue.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                returnValue.ErrorTitle = "Process Devices Request Failure.";

            }

            return returnValue;
        }

        public async Task<DeviceApiMultiRequestResponse> ProcessDevicesRequest(List<ApiRequest> apiRequests)
        {
            var returnValue = new DeviceApiMultiRequestResponse();

            try
            {
                var requestStr = JsonConvert.SerializeObject(apiRequests);

                var aRequestObject = JsonConvert.DeserializeObject<List<ApiRequest>>(requestStr, _jsonSerializerSettings);

                var response = await ProcessApiRequest(apiRequests);

                var apiResponseStr = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {                    

                    try
                    {
                        // Get the device data list if there is actually data.
                        var dataList = JsonConvert.DeserializeObject<List<DeviceData>>(apiResponseStr, _jsonSerializerSettings);
                        
                        returnValue.Data = dataList;
                        returnValue.Result = dataList.Select(x => x.Result == "success").Count() == apiRequests.Count() ? "success" : "failure";
                    }
                    catch
                    {
                        returnValue.ApiResponseStr = apiResponseStr ?? "No Response Content Returned.";

                        // See if this is an empty object (i.e. Asset Link returned empty arrays).
                        // If it's not an empty object list then this will exception out.
                        var dataList = JsonConvert.DeserializeObject<List<EmptyObject>>(apiResponseStr, _jsonSerializerSettings);

                        returnValue.Data = new List<Device.Models.DeviceData>();
                        returnValue.Result = returnValue.Result = dataList.Select(x => x.Result == "success").Count() == apiRequests.Count() ? "success" : "failure"; ;
                    }


                    returnValue.HttpStatusCode = (int)response.StatusCode;

                    returnValue.Success = true;
                }
                else
                {
                    returnValue.ApiResponseStr = apiResponseStr ?? "No Response Content Returned.";

                    returnValue.Result = apiResponseStr;

                    returnValue.HttpStatusCode = (int)response.StatusCode;

                    returnValue.Success = false;
                }

            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)
                _logger.Error($"Process Devices Request Failure", anException);

                returnValue.Success = false;
                returnValue.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                returnValue.ErrorCode = (int)anException.Call?.HttpStatus;
                returnValue.RequestProperties = new Dictionary<string, object>(anException.Call?.Request.Properties);
                returnValue.ErrorTitle = "Process Devices Request Failure.";


            }
            catch (Exception anException)
            {
                _logger.Error($"Process Devices Request Failure.", anException);

                returnValue.Success = false;
                returnValue.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                returnValue.ErrorTitle = "Process Devices Request Failure.";

            }

            return returnValue;
        }

        public async Task<GroupApiResponse> ProcessGroupsRequest(ApiRequest apiRequest)
        {
            GroupApiResponse returnValue = new GroupApiResponse();

            try
            {

                var response = await ProcessApiRequest(new List<ApiRequest> { apiRequest });
                var apiResponseStr = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    

                    List<GroupData> dataList = null;

                    try
                    {
                        // Get the device data list if there is actually data.
                        dataList = JsonConvert.DeserializeObject<List<GroupData>>(apiResponseStr, _jsonSerializerSettings);
                    }
                    catch
                    {
                        returnValue.ApiResponseStr = apiResponseStr ?? "No Response Content Returned.";

                        // See if this is an empty object (i.e. Asset Link returned empty arrays).
                        // If it's not an empty object list then this will exception out.
                        var emptyObjectList = JsonConvert.DeserializeObject<List<EmptyObject>>(apiResponseStr, _jsonSerializerSettings);

                        // return an empty list.
                        dataList = new List<GroupData>();
                    }

                    var data = dataList.FirstOrDefault();

                    returnValue.Data = data?.Data?.Groups;

                    returnValue.Result = data?.Result;

                    returnValue.HttpStatusCode = (int)response.StatusCode;

                    returnValue.Success = true;
                }
                else
                {
                    returnValue.ApiResponseStr = apiResponseStr ?? "No Response Content Returned.";

                    returnValue.Result = apiResponseStr;

                    returnValue.HttpStatusCode = (int)response.StatusCode;

                    returnValue.Success = false;
                }

            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)
                _logger.Error("Process Groups Request Failure.", anException);

                returnValue.Success = false;
                returnValue.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                returnValue.ErrorCode = (int)anException.Call?.HttpStatus;
                returnValue.RequestProperties = new Dictionary<string, object>(anException.Call?.Request.Properties);
                returnValue.ErrorTitle = "Process Groups Request Failure.";

            }
            catch (Exception anException)
            {
                _logger.Error("Process Groups Request Failure.", anException);

                returnValue.Success = false;
                returnValue.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                returnValue.ErrorTitle = "Process Groups Request Failure.";
            }


            return returnValue;
        }

        public async Task<MomentApiResponse> ProcessMomentsRequest(ApiRequest apiRequest)
        {
            MomentApiResponse returnValue = new MomentApiResponse();
            List<string> jsonErrors = new List<string>();
            bool jsonParsingFailed = false;
            string apiResponseStr = string.Empty;

            try
            {

                var response = await ProcessApiRequest(new List<ApiRequest> { apiRequest });
                apiResponseStr = await response.Content.ReadAsStringAsync();

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


                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                   
                    List<MomentData> dataList = null;

                    try
                    {
                        // Get the device data list if there is actually data.
                        dataList = JsonConvert.DeserializeObject<List<MomentData>>(apiResponseStr, jsonSerializerSettings);                       
                    }
                    catch
                    {
                        jsonParsingFailed = true;

                        returnValue.ApiResponseStr = apiResponseStr ?? "No Response Content Returned.";
                    }

                    if (jsonParsingFailed)
                    {
                        returnValue.ApiResponseStr = apiResponseStr ?? "No Response Content Returned.";

                        try
                        {
                            // See if this is an empty object (i.e. Asset Link returned empty arrays).
                            // If it's not an empty object list then this will exception out.
                            var emptyObjectList = JsonConvert.DeserializeObject<List<EmptyObject>>(apiResponseStr, jsonSerializerSettings);

                            jsonErrors.Clear();

                            var momentData = new MomentData
                            {
                                Result = "success",
                                Error = $"No Moment Data Returned.  API Request Filter:  {apiRequest.Filter}."
                            };

                            // return an empty list.
                            dataList = new List<MomentData> { momentData };
                        }
                        catch
                        {
                            var momentData = new MomentData
                            {
                                Result = "failure",
                                Error = $"Unable to parse non-empty API Response String."
                            };

                            // return an empty list.
                            dataList = new List<MomentData> { momentData };
                        }
                        
                    }

                    var data = dataList.FirstOrDefault();

                    returnValue.Data = data?.Data?.Moments;

                    returnValue.Result = data?.Result;

                    returnValue.HttpStatusCode = (int)response.StatusCode;

                    returnValue.Success = (returnValue.Result == "success") && (!jsonErrors.Any());

                    if (!returnValue.Success)
                    {
                        returnValue.ErrorTitle = "Moments Response Error.";
                        returnValue.ErrorMessage = $"AssetLink Error:  {data?.Error ?? "NONE"}  API Request Filter:  {apiRequest.Filter}  JSON Parsing Errors: {(!jsonErrors.Any() ? jsonErrors.Count : 0)}";
                        returnValue.ApiResponseStr = apiResponseStr;
                    }

                    returnValue.JsonParsingErrors = jsonErrors;
                }
                else
                {
                    returnValue.ApiResponseStr = apiResponseStr ?? "No Response Content Returned.";

                    returnValue.Result = apiResponseStr;

                    returnValue.HttpStatusCode = (int)response.StatusCode;

                    returnValue.Success = false;

                    returnValue.JsonParsingErrors = jsonErrors;
                }

            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)
                _logger.Error("Process Moments Request HTTP Failure.", anException);

                returnValue.Success = false;
                returnValue.ErrorCode = (int)anException.Call?.HttpStatus;
                returnValue.RequestProperties = new Dictionary<string, object>(anException.Call?.Request.Properties);
                returnValue.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                returnValue.ErrorTitle = "Process Moments Request HTTP Failure.";

            }
            catch (Exception anException)
            {
                _logger.Error("Process Moments Request Failure.", anException);

                returnValue.Success = false;
                returnValue.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                returnValue.ErrorTitle = "Process Moments Request Failure.";
                returnValue.ApiResponseStr = apiResponseStr;
                returnValue.JsonParsingErrors = jsonErrors;

            }

            return returnValue;
        }

        public async Task<DeviceProvisioningApiResponse> ProcessProvisioningRequest(ApiRequest apiRequest)
        {
            var returnValue = new DeviceProvisioningApiResponse();

            try
            {
                var response = await ProcessApiRequest(new List<ApiRequest> { apiRequest });
                var apiResponseStr = await response.Content?.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    

                    try
                    {
                        // Get the device data list if there is actually data.
                        var dataList = JsonConvert.DeserializeObject<List<DeviceProvisioningResultData>>(apiResponseStr, _jsonSerializerSettings);

                        var deviceResultData = dataList?.FirstOrDefault();

                        returnValue.Data = deviceResultData?.Data?.DevicesResults;
                        returnValue.Result = deviceResultData?.Result;
                    }
                    catch
                    {
                        returnValue.ApiResponseStr = apiResponseStr ?? "No Response Content Returned.";

                        // See if this is an empty object (i.e. Asset Link returned empty arrays).
                        // If it's not an empty object list then this will exception out.
                        var dataList = JsonConvert.DeserializeObject<List<EmptyObject>>(apiResponseStr, _jsonSerializerSettings);

                        var deviceData = dataList?.FirstOrDefault();

                        returnValue.Data = new List<DeviceProvisioningResult>();
                        returnValue.Result = deviceData?.Result;
                    }


                    returnValue.HttpStatusCode = (int)response.StatusCode;

                    returnValue.Success = true;
                }
                else
                {
                    returnValue.ApiResponseStr = apiResponseStr ?? "No Response Content Returned.";

                    returnValue.HttpStatusCode = (int)response.StatusCode;

                    returnValue.Success = false;
                }

            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)
                _logger.Error($"Process Devices Request Failure", anException);

                returnValue.Success = false;
                returnValue.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                returnValue.ErrorCode = (int)anException.Call?.HttpStatus;
                returnValue.RequestProperties = new Dictionary<string, object>(anException.Call?.Request.Properties);
                returnValue.ErrorTitle = "Process Devices Request Failure.";


            }
            catch (Exception anException)
            {
                _logger.Error($"Process Devices Request Failure.", anException);

                returnValue.Success = false;
                returnValue.ErrorMessage = $"{anException.Message} {anException.InnerException?.Message}";
                returnValue.ErrorTitle = "Process Devices Request Failure.";

            }

            return returnValue;
        }

        protected async Task<System.Net.Http.HttpResponseMessage> ProcessApiRequest(IList<ApiRequest> apiRequestList)
        {

            try
            {
                return await _apiConfig.BaseUrl
                              .AppendPathSegment(_apiConfig.ApiUrlPath)
                              .PostJsonAsync(apiRequestList);


            }
            catch (FlurlHttpTimeoutException anException)
            {

                // FlurlHttpTimeoutException derives from FlurlHttpException; catch here only
                // if you want to handle timeouts as a special case
                _logger.Error("Process API Request Failure.  Time Out.", anException);

                throw anException;
            }
            catch (FlurlHttpException anException)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)
                _logger.Error("Process API Request Failure.  Http Exception.", anException);

                throw anException;

            }
            catch (Exception anException)
            {
                _logger.Error("Process API Request Failure.", anException);

                throw anException;
            }

        }

    }
}
