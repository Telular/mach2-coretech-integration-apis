using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CommandLine;
using System.Threading;
using System.Threading.Tasks;
using AssetLinkGlobalApiLib;
using Common.Logging;

namespace AssetLinkBulkProvisioner
{
    using Common.Logging;
    using CsvHelper;
    using System.Globalization;

    class Program
    {
        private static readonly string appMutexId = "AssetLinkBulkProvisioner-1e5b3b6d-4507-44f8-a96e-0242d8c84cf6";

        private static readonly AssetLinkGlobalApi assetLinkGlobalApi = new AssetLinkGlobalApi();

        private static readonly ILog _logger = LogManager.GetLogger("AssetLinkBulkProvisioner");

        private static Mutex m_Mutex;

        private static ExitCode _exitCode = ExitCode.Success;


        static int Main(string[] args)
        {
            _logger.Info("Asset Link Global Bulk Provisioner is running.");                        

            if ((args.Length > 0))
            {
                // Block more than one instance of the application from running at a time (a singleton basically) to prevent multiple jobs of the same type overlapping each other.

                using (m_Mutex = new Mutex(true, appMutexId, out bool mutexObjectGranted))
                {

                    if (mutexObjectGranted)
                    {
                        try
                        {
                            var parseResult = Parser.Default.ParseArguments<CommandLineOptions>(args)
                            .WithParsed<CommandLineOptions>(options =>
                            {
                                // The CommandLineOptions class defines the variables and their corresponding command line options.
                                // Here we decode command line options that are legit (as in valid) options.
                                var aTask = Task.Run<ExitCode>(async () => await ProcessInputFile(options.InputFilename));

                                _exitCode = aTask.GetAwaiter().GetResult();

                            })
                            .WithNotParsed<CommandLineOptions>(errors =>
                            {
                                foreach (var error in errors)
                                {
                                    _exitCode = ExitCode.InvalidCommandLineArgs;
                                    _logger.Error($"Asset Link Global Bulk Provisioner - Invalid command line args.  Exit code:  {_exitCode}");
                                }


                            });


                            Console.WriteLine("Press Any Key....");

                            Console.ReadKey();


                        }
                        catch (Exception anException)
                        {
                            _logger.ErrorFormat("Asset Link Global Bulk Provisioner App Mutex.  App Mutex Id:  {0}  Exception:  {1} {2}", appMutexId, anException.Message, anException.InnerException == null ? string.Empty : anException.InnerException.Message);

                            _exitCode = ExitCode.GeneralException;
                        }
                        finally
                        {
                            m_Mutex.ReleaseMutex();
                        }

                    }
                    else
                    {
                        _logger.Error("Asset Link Global Bulk Provisioner - Unable to secure application mutex.");
                        _exitCode = ExitCode.NoMutexAvaliable;
                    }

                }
            }

            _logger.Info($"Asset Link Global Bulk Provisioner has completed.  Return code:  {_exitCode}");

            return (int)_exitCode;
        }

        static async Task<ExitCode> ProcessInputFile(string inputFilename)
        {
            var returnValue = ExitCode.Success;

            try
            {
                if (!File.Exists(inputFilename))
                {
                    _logger.Error($"Filename, {inputFilename}, does not exist.  Please use a valid filename with path.");
                    return ExitCode.InvalidFilename;
                }

                using (var reader = new StreamReader(inputFilename))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();

                    returnValue = await ProcessInputFile(csv);
                }

            }
            catch (Exception anException)
            {
                _logger.Error($"Unable to obtain CSV Reader.  Error:  {anException.Message} {anException?.InnerException?.Message}");
                returnValue = ExitCode.InvalidCsvReader;
            }


            return returnValue;
        }

        static async Task<ExitCode> ProcessInputFile(CsvReader csv)
        {
            _logger.Info("Processing Input file...");

            var returnValue = ExitCode.Success;

            if (csv == null)
            {
                _logger.Error($"Process Input File - Csv Reader is null.");
                return ExitCode.InvalidCsvReader;
            }

            int inputFileCounter = 0;
            
            while (csv.Read())
            {
                // Read the field values from the file.
                // NOTE:  Header titles are case sensitive which kinda sucks so make sure the file contains the"IMEI" column headers.     
                
                // Required - have to have the ESN.  After all that's what we're uploading.
                var hasESN = csv.TryGetField<string>("ESN", out string esn);

                try
                {
                    if (hasESN)
                    {
                        await ProvisionDevice(esn);

                        inputFileCounter++;
                    }

                }
                catch (Exception anException)
                {
                    _logger.Error($"Process Input File - General Exception on Row:  {csv.Context.Row} for ESN:  {esn}.  Exception: {anException.Message} {anException?.InnerException?.Message}", anException);
                    returnValue = ExitCode.GeneralException;

                    break;
                }

            }

            _logger.Info($"Asset Link Global Bulk Provisioner Line Count:  {inputFileCounter}");

            _logger.Info("Process Input File has completed.");
            return returnValue;
        }

        private static async Task ProvisionDevice(string deviceESN)
        {
            try
            {
                var loginRepsponse = await assetLinkGlobalApi.Login();
                

                var apiRequest = new ApiRequest
                {
                    File = AssetLinkGlobal.ApiFileIdentifiers.DEVICE,
                    Action = AssetLinkGlobal.ApiActionsIdentifiers.PROVISION,
                    Filter = $"esn = '{deviceESN}'"
                };

                var dataDictionary = new Dictionary<string, object>
                {
                    { "plan", AssetLinkParams.DefaultProvisionPlan }
                };

                apiRequest.Data = dataDictionary;

                var response = await assetLinkGlobalApi.ProcessProvisioningRequest(apiRequest);

                //var response = new AssetLinkGlobalApiLib.Device.DeviceProvisioningApiResponse { Success = true };

                if ((response == null) || !response.Success)
                {
                    _logger.Error($"Device Provisioning Response Failed:  {response.ErrorTitle} - {response.ErrorMessage}");
                }
                
                if (response.Data?.Any() == true)
                {
                    var aDeviceResult = response.Data.FirstOrDefault();

                    var aDevice = aDeviceResult.Device;

                    _logger.Info($"Device Properties ESN:  {aDevice.ESN} Name:  {aDevice.Name}  Plan {aDevice?.Extras?.Plan?.Name} Started:  {aDevice?.Extras?.Plan?.Started}  Prior Name:  {aDevice?.Extras?.Plan?.Priorname}  Prior Started: {aDevice?.Extras?.Plan?.Priorstarted}");
                }
                else
                {
                    _logger.Error($"Device Provisioning Failed.  No Data Returned for Device Properties ESN:  {deviceESN}");
                }
                
            }
            catch (Exception anException)
            {
                _logger.Error($"Provision Device With Plan Test Failure:  {anException.Message}  {(anException.InnerException != null ? anException.InnerException.Message : string.Empty)}");
            }
            finally
            {
                var logoutResponse = await assetLinkGlobalApi.Logout();

                if (!logoutResponse)
                {
                    _logger.Error("Failed to logout correctly.");
                }                
            }
        }
    }

    
}
