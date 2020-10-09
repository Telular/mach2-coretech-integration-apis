using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkyBitzApiLib.SkyBitzApi;
using SkyBitzApiLib.Models;
using System.Linq;

namespace SkyBitzApiTests
{
    [TestClass]
    public class SkyBitzApiTests
    {
        readonly SkyBitzApi skyBitzApi = new SkyBitzApi();

        [TestMethod]
        public async Task Test_PostTankReading_GoodShortId()
        {
            try
            {

                var newReading = new Dictionary<string, object>
                {
                    { "time", $"{DateTime.UtcNow.ToString("o")}" },
                    { "percent", 22.75 },
                    { "event_code", "thresh_1_trip" },
                    { "telular_test", true }                     
                };


                var postResponse = await skyBitzApi.PostTankReadingAsync(SkyBitzApiTestParams.GoodDeviceShortId, newReading);

                Assert.IsTrue(postResponse != null);

                Assert.IsTrue(postResponse.Success, $"POST Tank Reading Response Failed:  {postResponse.ErrorTitle} - {postResponse.ErrorMessage}");
               
            }
            catch (Exception anException)
            {
                Assert.Fail($"POST Tank Reading Exception:  {anException.Message}  {anException?.InnerException?.Message}");
            }
        }

        [TestMethod]
        public async Task Test_PostTankReading_GoodLongId()
        {
            try
            {

                var newReading = new Dictionary<string, object>
                {
                    { "time", $"{DateTime.UtcNow.ToString("o")}" },
                    { "percent", 22.75 },
                    { "event_code", "thresh_1_trip" },
                    { "telular_test", true }
                };


                var postResponse = await skyBitzApi.PostTankReadingAsync(SkyBitzApiTestParams.GoodDeviceLongId, newReading);

                Assert.IsTrue(postResponse != null);

                Assert.IsTrue(postResponse.Success, $"POST Tank Reading Response Failed:  {postResponse.ErrorTitle} - {postResponse.ErrorMessage}");

            }
            catch (Exception anException)
            {
                Assert.Fail($"POST Tank Reading Exception:  {anException.Message}  {anException?.InnerException?.Message}");
            }
        }

        [TestMethod]
        public async Task Test_PostTankReading_BadId()
        {
            try
            {

                var newReading = new Dictionary<string, object>
                {
                    { "time", $"{DateTime.UtcNow.ToString("o")}" },
                    { "percent", 22.75 },
                    { "event_code", "thresh_1_trip" },
                    { "telular_test", true }
                };


                var postResponse = await skyBitzApi.PostTankReadingAsync(SkyBitzApiTestParams.BadDeviceShortId, newReading);

                Assert.IsTrue(postResponse != null);

                Assert.IsTrue(postResponse.HttpStatusCode == 400, $"Response should be '400' not {postResponse.HttpStatusCode}.  {postResponse.ErrorTitle} - {postResponse.ErrorMessage}");

                Assert.IsFalse(postResponse.Success, $"POST Tank Reading Response Failed:  {postResponse.ErrorTitle} - {postResponse.ErrorMessage}");

            }
            catch (Exception anException)
            {
                Assert.Fail($"POST Tank Reading Exception:  {anException.Message}  {anException?.InnerException?.Message}");
            }
        }

        [TestMethod]
        public async Task Test_PostTankConfigChange_GoodShortId()
        {
            try
            {

                var newConfigChange = new Dictionary<string, object>
                {
                    { "state", $"deployed" },
                    { "address", "Chicago, ILL" },
                    { "capacity", 500 },
                    { "orientation", "vertical" },
                    { "description", "Vacation House!" },
                    { "reading_interval", 21600 },
                    { "transmission_interval", 86400 },
                    { "fixed_transmisssion_time", 25200 },
                    { "threshold_1", 30 },
                    { "threshold_2", 15 },
                    { "telular_test", true }
                };


                var postResponse = await skyBitzApi.PostDeviceConfigChangeAsync(SkyBitzApiTestParams.GoodDeviceShortId, newConfigChange);

                Assert.IsTrue(postResponse != null);

                Assert.IsTrue(postResponse.Success, $"POST Device Config Change Response Failed:  {postResponse.ErrorTitle} - {postResponse.ErrorMessage}");

            }
            catch (Exception anException)
            {
                Assert.Fail($"POST Device Config Change Exception:  {anException.Message}  {anException?.InnerException?.Message}");
            }
        }

        [TestMethod]
        public async Task Test_PostTankConfigChange_GoodLongId()
        {
            try
            {

                var newConfigChange = new Dictionary<string, object>
                {
                    { "state", $"deployed" },
                    { "address", "Chicago, ILL" },
                    { "capacity", 500 },
                    { "orientation", "vertical" },
                    { "description", "Vacation House!" },
                    { "reading_interval", 21600 },
                    { "transmission_interval", 86400 },
                    { "fixed_transmisssion_time", 25200 },
                    { "threshold_1", 30 },
                    { "threshold_2", 15 },
                    { "telular_test", true }
                };


                var postResponse = await skyBitzApi.PostDeviceConfigChangeAsync(SkyBitzApiTestParams.GoodDeviceLongId, newConfigChange);

                Assert.IsTrue(postResponse != null);

                Assert.IsTrue(postResponse.Success, $"POST Device Config Change Response Failed:  {postResponse.ErrorTitle} - {postResponse.ErrorMessage}");

            }
            catch (Exception anException)
            {
                Assert.Fail($"POST Device Config Change Exception:  {anException.Message}  {anException?.InnerException?.Message}");
            }
        }

        [TestMethod]
        public async Task Test_PostTankConfigChange_BadId()
        {
            try
            {

                var newConfigChange = new Dictionary<string, object>
                {
                    { "state", $"deployed" },
                    { "address", "Chicago, ILL" },
                    { "capacity", 500 },
                    { "orientation", "vertical" },
                    { "description", "Vacation House!" },
                    { "reading_interval", 21600 },
                    { "transmission_interval", 86400 },
                    { "fixed_transmisssion_time", 25200 },
                    { "threshold_1", 30 },
                    { "threshold_2", 15 },
                    { "telular_test", true }
                };


                var postResponse = await skyBitzApi.PostDeviceConfigChangeAsync(SkyBitzApiTestParams.BadDeviceShortId, newConfigChange);

                Assert.IsTrue(postResponse != null);

                Assert.IsTrue(postResponse.HttpStatusCode == 400, $"Response should be '400' not {postResponse.HttpStatusCode}.  {postResponse.ErrorTitle} - {postResponse.ErrorMessage}");

                Assert.IsFalse(postResponse.Success, $"POST Device Config Change Response Failed:  {postResponse.ErrorTitle} - {postResponse.ErrorMessage}");



            }
            catch (Exception anException)
            {
                Assert.Fail($"POST Device Config Change Exception:  {anException.Message}  {anException?.InnerException?.Message}");
            }
        }
    }
}
