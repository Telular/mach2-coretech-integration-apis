using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetLinkGlobalApiLib;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AssetLinkGlobalApiLib.AssetLinkGlobal;

namespace AssetLinkGlobalTests
{
    [TestClass]
    public class AssetLinkGlobalApiTests
    {
        private readonly AssetLinkGlobalApi assetLinkGlobalApi = new AssetLinkGlobalApi();        

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task Login_LogoutTest()
        {
            try
            {
                var response = await assetLinkGlobalApi.Login();

                Assert.IsTrue(response.Success, "Failed to Login.");
            }
            catch (Exception anException)
            {
                Assert.Fail($"Login Logout Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var response = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(response, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [Ignore]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task ProvisionDevice_WithPlan()
        {
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.DEVICE,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.PROVISION,
                    Filter = $"esn = '{300534060028020}'"
                };

                var dataDictionary = new Dictionary<string, object>
                {
                    { "plan", AssetLinkGlobalTestParams.AssetLinkDefaultProvisionPlan }
                };

                apiRequest.Data = dataDictionary;

                var response = await assetLinkGlobalApi.ProcessProvisioningRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Device Provisioning Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

                var aDeviceResult = response.Data.FirstOrDefault();

                Assert.IsNotNull(aDeviceResult);
                
                Assert.IsNotNull(aDeviceResult.Device);
            }
            catch (Exception anException)
            {
                Assert.Fail($"Provision Device With Plan Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [Ignore]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task ProvisionDevice_WithNoPlan()
        {
            // The test and command are legit however currently if "no plan" is specified it defaults to the 
            // SBD Track 3.2 KB plan which, for Telular, does not link to a "billing plan".  Which is not good.
            // Asset Link is making a software change so that in the future it will default to the "plan" negotiated by the company.
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.DEVICE,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.PROVISION,
                    Filter = $"esn = '{AssetLinkGlobalTestParams.GoodEsnToo}'"
                };
               
                var response = await assetLinkGlobalApi.ProcessProvisioningRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Device Provisioning Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

                var aDeviceResult = response.Data.FirstOrDefault();

                Assert.IsNotNull(aDeviceResult);
                
                Assert.IsNotNull(aDeviceResult.Device);
            }
            catch (Exception anException)
            {
                Assert.Fail($"Provision Device With Plan Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [Ignore]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task DeProvisionDevice()
        {
            // Use with absolute CAUTION.  
            // This de-provisions a device on AssetLink.  The plan is removed and the device is set to DEACTIVE.
            // Do NOT remove the "Ignore" tag unless you plan on de-provisioning a device manual.  Use the Carrier API.
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.DEVICE,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.DEPROVISION,
                    Filter = $"esn = '{AssetLinkGlobalTestParams.GoodEsnToo}'"
                };

                var response = await assetLinkGlobalApi.ProcessProvisioningRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Device De-Provisioning Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

                var aDeviceResult = response.Data.FirstOrDefault();

                Assert.IsNotNull(aDeviceResult);

                Assert.IsNotNull(aDeviceResult.Device);
            }
            catch (Exception anException)
            {
                Assert.Fail($"De-Provision Device Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetDevices_UnFiltered()
        {
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.DEVICE,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Limit = 5
                };


                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Device Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");

                Assert.IsNotNull(response.Data);

                Assert.IsTrue(response.Data.Any());

                
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Devices UnFiltered Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetDevices_Filtered()
        {
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.DEVICE,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Filter = $"esn = '{300534061244450}'"
                };


                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Device Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Count() == 1);
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Devices Filtered Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetDevices_Filtered_BadESN()
        {
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.DEVICE,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Filter = $"esn = '{AssetLinkGlobalTestParams.BadEsn}'"
                };


                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Device Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Count == 0);
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Devices Filtered Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetDevices_Unfiltered_Then_Filtered()
        {
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                var apiRequestMultiDevice = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.DEVICE,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Limit = 5
                };


                var responseMultiDevice = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequestMultiDevice);

                var aDevice = responseMultiDevice?.Data?.FirstOrDefault();

                Assert.IsNotNull(aDevice, "No device was returned from the multi-device API request.");

                var apiRequestSingleDevice = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.DEVICE,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Filter = $"esn = '{aDevice.ESN}'"
                };

                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequestSingleDevice);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Device Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Count() == 1);

                var returnedDevice = response.Data.FirstOrDefault();

                Assert.IsTrue(returnedDevice.ESN == aDevice.ESN, $"The returned device ESN {returnedDevice.ESN} does not match the request device ESN {aDevice.ESN}.");
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Devices Filtered Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetRemoteReport_GoodEsn()
        {
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                // Get device information via a "remote" reporting request which returns device information.
                // NOTE:  A "remote" report request will fail if you don't specify a subset of devices.  This is an "over the air" direct to device sort of thing
                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.REMOTE,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.REPORT,
                    Filter = $"esn = '{AssetLinkGlobalTestParams.GoodEsn}'"
                };


                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Remote Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Remote Report Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task SetRemoteDeviceMTData_SpiFlashEsn_SMS_Message_1()
        {
            try
            {
                // This unit test creates "spiflash" action requests for each string that comes back from the SMS message convertor.
                // It passes the list of requests to an overloaded copy of the ProcessDevicesRequest method.
                // AssetLink's API by default handles multiple request calls in one server (http request) call.
                var loginRepsponse = await assetLinkGlobalApi.Login();

                var smsMessage = ">35000000313B9AC9ED000000000D82800500FFFFFFFFFFFFFF0212000583927F3CB4001284BA0000000000000000001E0A100000C8C84DE7";

                var dataList = SmsUtlity.SmsToAssetLinkApiString(smsMessage);

                var apiRequestList = new List<ApiRequest>();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                // SMS Convertor will split the SMS Message into 64 byte chunks (max data size per request).
                foreach (var strData in dataList)
                {
                    // Send a Device an MT message via the Remote request which returns device information.               
                    var apiRequest = new ApiRequest
                    {
                        File = AssetLinkGlobal.ApiFileIdentifiers.REMOTE,
                        Action = AssetLinkGlobal.ApiActionsIdentifiers.SPIFLASH,
                        Filter = $"esn = '{AssetLinkGlobalTestParams.SpiFlashESN}'"
                    };

                    apiRequest.Data = strData;

                    apiRequestList.Add(apiRequest);
                }

                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequestList);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Remote Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Remote Report Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task SetRemoteDeviceMTData_SpiFlashEsn_SMS_Message_2()
        {
            try
            {
                // This unit test creates "spiflash" action requests for each string that comes back from the SMS message convertor.
                // It passes the list of requests to an overloaded copy of the ProcessDevicesRequest method.
                // AssetLink's API by default handles multiple request calls in one server (http request) call.
                var loginRepsponse = await assetLinkGlobalApi.Login();

                var smsMessage = ">4d000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e";

                var dataList = SmsUtlity.SmsToAssetLinkApiString(smsMessage);

                var apiRequestList = new List<ApiRequest>();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                // SMS Convertor will split the SMS Message into 64 byte chunks (max data size per request).
                foreach (var strData in dataList)
                {
                    // Send a Device an MT message via the Remote request which returns device information.               
                    var apiRequest = new ApiRequest
                    {
                        File = AssetLinkGlobal.ApiFileIdentifiers.REMOTE,
                        Action = AssetLinkGlobal.ApiActionsIdentifiers.SPIFLASH,
                        Filter = $"esn = '{AssetLinkGlobalTestParams.SpiFlashESN}'"
                    };

                    apiRequest.Data = strData;

                    apiRequestList.Add(apiRequest);
                }

                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequestList);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Remote Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Remote Report Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task SetRemoteDeviceMTData_SpiFlashEsn_SMS_Message_3()
        {
            try
            {
                // This unit test creates "spiflash" action requests for each string that comes back from the SMS message convertor.
                // It passes the list of requests to an overloaded copy of the ProcessDevicesRequest method.
                // AssetLink's API by default handles multiple request calls in one server (http request) call.
                var loginRepsponse = await assetLinkGlobalApi.Login();

                var smsMessage = ">4d4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d";

                var dataList = SmsUtlity.SmsToAssetLinkApiString(smsMessage);

                var apiRequestList = new List<ApiRequest>();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                // SMS Convertor will split the SMS Message into 64 byte chunks (max data size per request).
                foreach (var strData in dataList)
                {
                    // Send a Device an MT message via the Remote request which returns device information.               
                    var apiRequest = new ApiRequest
                    {
                        File = AssetLinkGlobal.ApiFileIdentifiers.REMOTE,
                        Action = AssetLinkGlobal.ApiActionsIdentifiers.SPIFLASH,
                        Filter = $"esn = '{AssetLinkGlobalTestParams.SpiFlashESN}'"
                    };

                    apiRequest.Data = strData;

                    apiRequestList.Add(apiRequest);
                }

                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequestList);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Remote Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Remote Report Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task SetRemoteDeviceMTData_SpiFlashEsn_SMS_Message_4()
        {
            try
            {
                // This unit test creates "spiflash" action requests for each string that comes back from the SMS message convertor.
                // It passes the list of requests to an overloaded copy of the ProcessDevicesRequest method.
                // AssetLink's API by default handles multiple request calls in one server (http request) call.
                var loginRepsponse = await assetLinkGlobalApi.Login();

                var smsMessage = ">4d9e9fa0a1a2a3a4a5a6a7a8a9aaabacadaeafb0b1b2b3b4b5b6b7b8b9babbbcbdbebfc0c1c2c3c4c5c6c7c8c9cacbcccdcecfd0d1d2d3d4d5d6d7d8d9dadbdcdddedfe0e1e2e3e4e5e6e7e8e9eaebec";

                var dataList = SmsUtlity.SmsToAssetLinkApiString(smsMessage);

                var apiRequestList = new List<ApiRequest>();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                // SMS Convertor will split the SMS Message into 64 byte chunks (max data size per request).
                foreach (var strData in dataList)
                {
                    // Send a Device an MT message via the Remote request which returns device information.               
                    var apiRequest = new ApiRequest
                    {
                        File = AssetLinkGlobal.ApiFileIdentifiers.REMOTE,
                        Action = AssetLinkGlobal.ApiActionsIdentifiers.SPIFLASH,
                        Filter = $"esn = '{AssetLinkGlobalTestParams.SpiFlashESN}'"
                    };

                    apiRequest.Data = strData;

                    apiRequestList.Add(apiRequest);
                }

                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequestList);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Remote Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Remote Report Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task SetRemoteDeviceMTData_SpiFlashEsn_SMS_Message_5()
        {
            try
            {
                // This unit test creates "spiflash" action requests for each string that comes back from the SMS message convertor.
                // It passes the list of requests to an overloaded copy of the ProcessDevicesRequest method.
                // AssetLink's API by default handles multiple request calls in one server (http request) call.
                var loginRepsponse = await assetLinkGlobalApi.Login();

                var smsMessage = ">11edeeeff0f1f2f3f4f5f6f7f8f9fafbfcfdfeff";

                var dataList = SmsUtlity.SmsToAssetLinkApiString(smsMessage);

                var apiRequestList = new List<ApiRequest>();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                // SMS Convertor will split the SMS Message into 64 byte chunks (max data size per request).
                foreach (var strData in dataList)
                {
                    // Send a Device an MT message via the Remote request which returns device information.               
                    var apiRequest = new ApiRequest
                    {
                        File = AssetLinkGlobal.ApiFileIdentifiers.REMOTE,
                        Action = AssetLinkGlobal.ApiActionsIdentifiers.SPIFLASH,
                        Filter = $"esn = '{AssetLinkGlobalTestParams.SpiFlashESN}'"
                    };

                    apiRequest.Data = strData;

                    apiRequestList.Add(apiRequest);
                }

                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequestList);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Remote Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Remote Report Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task SetRemoteDeviceMTData_SpiFlashEsn_SMS_Message_6()
        {
            try
            {
                // This unit test creates "spiflash" action requests for each string that comes back from the SMS message convertor.
                // It passes the list of requests to an overloaded copy of the ProcessDevicesRequest method.
                // AssetLink's API by default handles multiple request calls in one server (http request) call.
                var loginRepsponse = await assetLinkGlobalApi.Login();

                var smsMessage = ">4dffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";

                var dataList = SmsUtlity.SmsToAssetLinkApiString(smsMessage);

                var apiRequestList = new List<ApiRequest>();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                // SMS Convertor will split the SMS Message into 64 byte chunks (max data size per request).
                foreach (var strData in dataList)
                {
                    // Send a Device an MT message via the Remote request which returns device information.               
                    var apiRequest = new ApiRequest
                    {
                        File = AssetLinkGlobal.ApiFileIdentifiers.REMOTE,
                        Action = AssetLinkGlobal.ApiActionsIdentifiers.SPIFLASH,
                        Filter = $"esn = '{AssetLinkGlobalTestParams.SpiFlashESN}'"
                    };

                    apiRequest.Data = strData;

                    apiRequestList.Add(apiRequest);
                }

                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequestList);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Remote Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Remote Report Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [Ignore]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task SetRemoteDeviceMTData_SpiFlashEsn_SMS_Message_MReuter()
        {
            try
            {
                // This unit test creates "spiflash" action requests for each string that comes back from the SMS message convertor.
                // It passes the list of requests to an overloaded copy of the ProcessDevicesRequest method.
                // AssetLink's API by default handles multiple request calls in one server (http request) call.
                var loginRepsponse = await assetLinkGlobalApi.Login();

                var smsMessage = ">3A000006363B9AC9D4000000000302A201000D76800600FFFFFFFFFFFFFF060400057792013CB4001278BA0000000000000000001E1F100000C8C810F0";

                var dataList = SmsUtlity.SmsToAssetLinkApiString(smsMessage);

                var apiRequestList = new List<ApiRequest>();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                // SMS Convertor will split the SMS Message into 64 byte chunks (max data size per request).
                foreach (var strData in dataList)
                {
                    // Send a Device an MT message via the Remote request which returns device information.               
                    var apiRequest = new ApiRequest
                    {
                        File = AssetLinkGlobal.ApiFileIdentifiers.REMOTE,
                        Action = AssetLinkGlobal.ApiActionsIdentifiers.SPIFLASH,
                        Filter = $"esn = '{AssetLinkGlobalTestParams.SpiFlashESN_MReuter}'"
                    };

                    apiRequest.Data = strData;

                    apiRequestList.Add(apiRequest);
                }

                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequestList);                

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Remote Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Remote Report Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }


        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetRemoteReport_BadEsn()
        {
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                // Get device information via a "remote" reporting request which returns device information.
                // NOTE:  A "remote" report request will fail if you don't specify a subset of devices.  This is an "over the air" direct to device sort of thing
                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.REMOTE,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.REPORT,
                    Filter = $"esn = '{AssetLinkGlobalTestParams.BadEsn}'"
                };


                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Remote Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Count() == 0);
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Remote Report Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetRemoteReport_NoEsn()
        {
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                // Get device information via a "remote" reporting request which returns device information.
                // NOTE:  A "remote" report request will fail if you don't specify a subset of devices.  This is an "over the air" direct to device sort of thing
                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.REMOTE,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.REPORT
                };


                var response = await assetLinkGlobalApi.ProcessDevicesRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Remote Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data == null);

                Assert.IsTrue(response.Result == "error", "The internal result should have returned  'error'.  Asset Link doesn't allow calls to remote WITHOUT a filter specified.");
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Remote Report Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetGroups_UnFiltered()
        {
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.GROUP,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Limit = 5
                };


                var response = await assetLinkGlobalApi.ProcessGroupsRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Group Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Groups Unfiltered Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetGroups_Filtered()
        {

            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.GROUP,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Filter = $"(Grp = '{AssetLinkGlobalTestParams.GoodGroup}')"
                };


                var response = await assetLinkGlobalApi.ProcessGroupsRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Group Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Count() == 1);
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Groups Filtered Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetGroups_UnFiltered_Then_Filtered()
        {

            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                var apiMultiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.GROUP,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Limit = 5
                };


                var responseMulti = await assetLinkGlobalApi.ProcessGroupsRequest(apiMultiRequest);

                var aGroup = responseMulti?.Data?.FirstOrDefault();

                Assert.IsNotNull(aGroup, "No group was returned from the multi-device API request.");

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.GROUP,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Filter = $"(Grp = {aGroup.GroupId})"
                };


                var response = await assetLinkGlobalApi.ProcessGroupsRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Group Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Count() == 1);
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Groups Filtered Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetMoments_UnFiltered()
        {
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.MOMMENT,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Limit = 10
                };

                var response = await assetLinkGlobalApi.ProcessMomentsRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Moment Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Moments Unfiltered Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetMoments_Filtered()
        {

            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                // Get just one moment.

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.MOMMENT,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Filter = $"(Moment > {AssetLinkGlobalTestParams.GoodMomentIdForMoData})",
                    Limit = 10
                };

                var response = await assetLinkGlobalApi.ProcessMomentsRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Moment Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");

                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Moments Filtered Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetMoments_MO_Data_Filtered()
        {

            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                // Get just one moment.

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.MOMMENT,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Filter = $"(esn = '{300534061142140}')",
                    Limit = 15
                };

                var response = await assetLinkGlobalApi.ProcessMomentsRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Moment Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");

                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Moments Filtered Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }


        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetMoments_UnFiltered_Then_Filtered()
        {

            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                var apiMultiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.MOMMENT,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Limit = 5
                };

                var responseMulti = await assetLinkGlobalApi.ProcessMomentsRequest(apiMultiRequest);

                var moment = responseMulti?.Data?.FirstOrDefault();

                Assert.IsNotNull(moment, "No moment data was returned from the multi-device API request.");

                var momentElements = moment?.Moments;

                var aMomentElement = momentElements?.FirstOrDefault();

                Assert.IsNotNull(aMomentElement, "No moment element was returned from the multi-device API request.");


                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.MOMMENT,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Filter = $"(Moment > {aMomentElement.Momentid})",
                    Limit = 10
                };

                var response = await assetLinkGlobalApi.ProcessMomentsRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Moment Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");

                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Moments Filtered Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.CATEGORY_UNIT)]
        public async Task GetMoments_MO_Data_Filtered_InBulk()
        {
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();

                Assert.IsTrue(loginRepsponse.Success, "Failed to Login.");

                // Get just one moment.

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.MOMMENT,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.GET,
                    Filter = $"(Moment > {AssetLinkGlobalTestParams.GoodMomentIdForMoData})",
                    Limit = 1000
                };

                var response = await assetLinkGlobalApi.ProcessMomentsRequest(apiRequest);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Moment Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");

                Assert.IsTrue(response.Data != null);

                Assert.IsTrue(response.Data.Any());

                foreach (var momentData in response.Data)
                {
                    Console.WriteLine($"Device ID:  {momentData.Deviceid}  ESN:  {momentData.Esn}  Last TX Time: {momentData.Lasttxtime}");

                    foreach(var momentElement in momentData.Moments)
                    {
                        string[] numArray = new string[] { "3", "4" };
                        bool lookforPayload = false;

                        Console.WriteLine($"\nMoment ID:  {momentElement.Momentid}  Origination Date:  {momentElement.DateOriginated}  Received Date:  {momentElement.DateReceived}  Reported Date:  {momentElement.DateReported}");

                        foreach (var aPoint in momentElement?.Points)
                        {

                            var aPointType = (string)GetFieldValue(MomentPointTypes.POINT_TYPE_KEYNAME, aPoint.DataPoint);                            

                            // Checking the MsgType - we're interested in "3" or "4"
                            if (aPointType == MomentPointTypes.POINT_MSGTYPE)
                            {
                                var msgType = GetFieldValue("MsgType", aPoint.DataPoint);

                                var numStr = GetFieldValue("num", aPoint.DataPoint);

                                lookforPayload = numArray.Contains(numStr);

                                int number = (lookforPayload && int.TryParse(numStr, out int outValue)) ? outValue : 0;
                               
                                Console.WriteLine($"Point Type:  {aPointType}  Msg Type:  {msgType}  Num:  {number}");

                                continue;
                            }
                            // If there's a MsgType of "3" or "4" then we have a valid payload.
                            else if (lookforPayload && (aPointType == MomentPointTypes.POINT_PAYLOAD))
                            {
                                var payload = GetFieldValue("payload", aPoint.DataPoint);
                                                                
                                Console.WriteLine($"Point Type:  {aPointType}  Payload:  {payload}\n");

                                lookforPayload = false;

                                continue;
                            }
                            //else
                            //{
                            //    // For all the other points that get carried along in the stream.
                            //    Console.WriteLine($"Point Type:  {aPointType}");
                            //    foreach( var aKey in aPoint.DataPoint.Keys)
                            //    {
                            //        if (aKey == MomentPointTypes.POINT_TYPE_KEYNAME) continue;
                            //        Console.WriteLine($"Key:  {aKey}  Value:  {aPoint.DataPoint[aKey]}");
                            //    }

                            //    Console.WriteLine();
                            //}


                        }
                    }
                }

            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Moments Filtered Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                Assert.IsTrue(logoutResponse, "Failed to logout correctly.");
            }
        }

        private string GetFieldValue(string keyName, Dictionary<string, object> device)
        {
            if (device.ContainsKey(keyName))
            {
                return device[keyName]?.ToString() ?? string.Empty;
            }
            return string.Empty;
        }
    }
}
