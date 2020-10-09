using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TankUtilityApiLib.TankUtilityApiBasicAuth;
using TankUtilityApiLib.Models;
using System.Linq;

namespace TankUtilityApiTests
{
    

    [TestClass]
    public class TankUtilityApiBasicAuthTests
    {
        readonly TankUtilityApiBasicAuth tankUtilityApi = new TankUtilityApiBasicAuth();

        [TestMethod]
        public async Task Test_GetDevicesAsync()
        {
            try
            {

                var response = await tankUtilityApi.GetDevicesAsync();

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Get Devices Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                var devicesResponse = response.Data as DevicesResponse;

                Assert.IsNotNull(devicesResponse);

                Assert.IsNotNull(devicesResponse.Devices);

                Assert.IsTrue(devicesResponse.Devices.Any());
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Devices Exception:  {anException.Message}  {anException?.InnerException?.Message}");
            }
        }

        [TestMethod]
        public async Task Test_GetDeviceAsync()
        {
            try
            {

                var response = await tankUtilityApi.GetDeviceAsync(TankUtilityApiTestParams.MikeReuterBlaineDevice);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Get Device Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                var deviceResponse = response.Data as DeviceResponse;

                Assert.IsNotNull(deviceResponse);

                var device = deviceResponse.Device;

                Assert.IsNotNull(device);
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Device Test Failure:  {anException.Message}  {anException?.InnerException?.Message}");
            }

        }

        [TestMethod]
        public async Task Test_GetDeviceReadingsAsync()
        {
            try
            {

                var response = await tankUtilityApi.GetDeviceReadingsAsync(TankUtilityApiTestParams.MikeReuterBlaineDevice);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Get Device Readings Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                var devicesReadingsResponse = response.Data as DeviceReadingsResponse;

                Assert.IsNotNull(devicesReadingsResponse);

                Assert.IsTrue(devicesReadingsResponse.Any());
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Device Readings Test Failure:  {anException.Message}  {anException?.InnerException?.Message}");
            }

        }


        [Ignore]
        [TestMethod]
        public async Task Test_PatchDeviceConfigAsync()
        {
            try
            {

                var response = await tankUtilityApi.GetDeviceAsync(TankUtilityApiTestParams.GoodDeviceShortId);

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Get Devices Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                var deviceResponse = response.Data as DeviceResponse;

                Assert.IsNotNull(deviceResponse);

                var device = deviceResponse.Device;

                Assert.IsNotNull(device);

                var newConfig = new Dictionary<string, object>
                {
                    { "address", $"{TankUtilityApiTestParams.GoodAddressForGAREVKMJ} {DateTime.UtcNow.ToString("o")}" }
                     //{ "reading_interval", 10800} 
                };

                var postResponse = await tankUtilityApi.PatchDeviceConfigAsync(TankUtilityApiTestParams.GoodDeviceShortId, newConfig);

                Assert.IsTrue(postResponse != null);

                Assert.IsTrue(postResponse.Success, $"Get Devices Response Failed:  {postResponse.ErrorTitle} - {postResponse.ErrorMessage}");

            }
            catch (Exception anException)
            {
                Assert.Fail($"Post Device Configuration Exception:  {anException.Message}  {anException?.InnerException?.Message}");
            }
        }

        [TestMethod]
        public async Task Test_GetDeviceAllAsync()
        {
            try
            {

                var response = await tankUtilityApi.GetDevicesAsync();

                Assert.IsTrue(response != null);

                Assert.IsTrue(response.Success, $"Get Devices Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                Assert.IsTrue(response.Data != null);

                var devicesResponse = response.Data as DevicesResponse;

                Assert.IsNotNull(devicesResponse);

                Assert.IsNotNull(devicesResponse.Devices);

                Assert.IsTrue(devicesResponse.Devices.Any());

                foreach (var aDeviceLongId in devicesResponse.Devices)
                {
                    var responseEachDevice = await tankUtilityApi.GetDeviceAsync(aDeviceLongId);

                    Assert.IsTrue(responseEachDevice != null);

                    Assert.IsTrue(responseEachDevice.Success, $"Get Device Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                    Assert.IsTrue(responseEachDevice.Data != null);

                    var deviceResponse = responseEachDevice.Data as DeviceResponse;

                    Assert.IsNotNull(deviceResponse);

                    var device = deviceResponse.Device;

                    Assert.IsNotNull(device);

                }
            }
            catch (Exception anException)
            {
                Assert.Fail($"Get Devices Exception:  {anException.Message}  {anException?.InnerException?.Message}");
            }
        }


    }
}
