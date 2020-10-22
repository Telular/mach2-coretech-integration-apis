﻿using System;
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

        [TestMethod]
        public async Task Test_PostTankReading_LoopTest()
        {
            double readingPercent = 22.75;

            try
            {
                var newReading = new Dictionary<string, object>
                {
                    { "time", $"{DateTime.UtcNow.ToString("o")}" },
                    { "percent", readingPercent },
                    { "event_code", "thresh_1_trip" },
                    { "telular_test", true }
                };

                for (int i = 0; i < 5000; i++)
                {
                    readingPercent += .01;

                    newReading["time"] = $"{DateTime.UtcNow:o}";
                    newReading["percent"] = readingPercent;

                    var postResponse = await skyBitzApi.PostTankReadingAsync(SkyBitzApiTestParams.GoodDeviceShortId, newReading);

                    Assert.IsTrue(postResponse != null);

                    Assert.IsTrue(postResponse.Success, $"POST Tank Reading Response Failed:  {postResponse.ErrorTitle} - {postResponse.ErrorMessage}");
                }

            }
            catch (Exception anException)
            {
                Assert.Fail($"POST Tank Reading Exception:  {anException.Message}  {anException?.InnerException?.Message}");
            }
        }

        [TestMethod]
        public async Task Test_PostTankConfigChange_LoopTest()
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

                for (int i = 0; i < 5000; i++)
                {
                    var postResponse = await skyBitzApi.PostDeviceConfigChangeAsync(SkyBitzApiTestParams.GoodDeviceShortId, newConfigChange);

                    Assert.IsTrue(postResponse != null);

                    Assert.IsTrue(postResponse.Success, $"POST Device Config Change Response Failed:  {postResponse.ErrorTitle} - {postResponse.ErrorMessage}");
                }
            }
            catch (Exception anException)
            {
                Assert.Fail($"POST Device Config Change Exception:  {anException.Message}  {anException?.InnerException?.Message}");
            }

        }

        [TestMethod]
        public async Task Test_PostBothEndpoints_LoopTest()
        {
            double readingPercent = 22.75;

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

                var newReading = new Dictionary<string, object>
                {
                    { "time", $"{DateTime.UtcNow.ToString("o")}" },
                    { "percent", readingPercent },
                    { "event_code", "thresh_1_trip" },
                    { "telular_test", true }
                };

                for (int i = 0; i < 5000; i++)
                {
                    readingPercent += .01;

                    newReading["time"] = $"{DateTime.UtcNow:o}";
                    newReading["percent"] = readingPercent;

                    var readingPostResponse = await skyBitzApi.PostTankReadingAsync(SkyBitzApiTestParams.GoodDeviceShortId, newReading);                    

                    Assert.IsTrue(readingPostResponse?.Success == true, $"POST Tank Reading Response Failed:  {readingPostResponse.ErrorTitle} - {readingPostResponse.ErrorMessage}");

                    var configPostResponse = await skyBitzApi.PostDeviceConfigChangeAsync(SkyBitzApiTestParams.GoodDeviceShortId, newConfigChange);                    

                    Assert.IsTrue(configPostResponse?.Success == true, $"POST Device Config Change Response Failed:  {configPostResponse.ErrorTitle} - {configPostResponse.ErrorMessage}");
                }
            }
            catch (Exception anException)
            {
                Assert.Fail($"POST Device Config Change Exception:  {anException.Message}  {anException?.InnerException?.Message}");
            }

        }

        [TestMethod]
        public void Test_PostBothEndpoints_LoopTest2()
        {
            double readingPercent = 22.75;

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

                var newReading = new Dictionary<string, object>
                {
                    { "time", $"{DateTime.UtcNow.ToString("o")}" },
                    { "percent", readingPercent },
                    { "event_code", "thresh_1_trip" },
                    { "telular_test", true }
                };

                for (int i = 0; i < 5000; i++)
                {
                    readingPercent += .01;

                    newReading["time"] = $"{DateTime.UtcNow:o}";
                    newReading["percent"] = readingPercent;

                    var readingTask1 = skyBitzApi.PostTankReadingAsync(SkyBitzApiTestParams.GoodDeviceShortId, newReading);
                    var readingTask2 = skyBitzApi.PostTankReadingAsync(SkyBitzApiTestParams.GoodDeviceLongId, newReading);
                    var readingTask3 = skyBitzApi.PostTankReadingAsync(SkyBitzApiTestParams.GoodDeviceShortId, newReading);
                    var readingTask4 = skyBitzApi.PostTankReadingAsync(SkyBitzApiTestParams.GoodDeviceLongId, newReading);

                    var configTask1 = skyBitzApi.PostDeviceConfigChangeAsync(SkyBitzApiTestParams.GoodDeviceShortId, newConfigChange);
                    var configTask2 = skyBitzApi.PostDeviceConfigChangeAsync(SkyBitzApiTestParams.GoodDeviceLongId, newConfigChange);
                    var configTask3 = skyBitzApi.PostDeviceConfigChangeAsync(SkyBitzApiTestParams.GoodDeviceShortId, newConfigChange);
                    var configTask4 = skyBitzApi.PostDeviceConfigChangeAsync(SkyBitzApiTestParams.GoodDeviceLongId, newConfigChange);

                    Task.WaitAll(new Task[] { readingTask1, readingTask2, readingTask3, readingTask4, configTask1,  configTask2, configTask3, configTask4 });

                  

                    Assert.IsTrue(readingTask1.Result?.Success == true, $"POST Tank Reading Response Failed:  {readingTask1.Result?.ErrorTitle} - {readingTask1.Result?.ErrorMessage}");
                    Assert.IsTrue(readingTask2.Result?.Success == true, $"POST Tank Reading Response Failed:  {readingTask2.Result?.ErrorTitle} - {readingTask2.Result?.ErrorMessage}");
                    Assert.IsTrue(readingTask3.Result?.Success == true, $"POST Tank Reading Response Failed:  {readingTask3.Result?.ErrorTitle} - {readingTask3.Result?.ErrorMessage}");
                    Assert.IsTrue(readingTask4.Result?.Success == true, $"POST Tank Reading Response Failed:  {readingTask4.Result?.ErrorTitle} - {readingTask4.Result?.ErrorMessage}");

                    Assert.IsTrue(configTask1.Result?.Success == true, $"POST Device Config Change Response Failed:  {configTask1.Result?.ErrorTitle} - {configTask1.Result?.ErrorMessage}");
                    Assert.IsTrue(configTask2.Result?.Success == true, $"POST Device Config Change Response Failed:  {configTask2.Result?.ErrorTitle} - {configTask2.Result?.ErrorMessage}");
                    Assert.IsTrue(configTask3.Result?.Success == true, $"POST Device Config Change Response Failed:  {configTask3.Result?.ErrorTitle} - {configTask3.Result?.ErrorMessage}");
                    Assert.IsTrue(configTask4.Result?.Success == true, $"POST Device Config Change Response Failed:  {configTask4.Result?.ErrorTitle} - {configTask4.Result?.ErrorMessage}");
                }
            }
            catch (Exception anException)
            {
                Assert.Fail($"POST Device Config Change Exception:  {anException.Message}  {anException?.InnerException?.Message}");
            }

        }
    }
}
