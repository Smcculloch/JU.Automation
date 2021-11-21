using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Services
{
    public interface ISetupActionService
    {
        Task NewDeveloper();
        Task<SearchResult> GetNewLights();
        Task<SearchResult> SearchNewLights();
        Task<bool> RunInitialSetup();
    }

    public class SearchResult
    {
        public bool Success { get; set; }
        public string[] Errors { get; set; }
        public int LightsFound { get; set; }
    }

    public class SetupActionService : ISetupActionService
    {
        private readonly IHueClient _hueClient;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IEnumerable<ISetupAction> _setupActions;

        public SetupActionService(
            IHueClient hueClient,
            ISettingsProvider settingsProvider,
            IEnumerable<ISetupAction> setupActions)
        {
            _hueClient = hueClient;
            _settingsProvider = settingsProvider;
            _setupActions = setupActions.Cast<IStep>()
                                        .OrderBy(actionStep => actionStep.Step)
                                        .Cast<ISetupAction>();
        }

        public async Task NewDeveloper()
        {
            Console.Write("Enter application name: ");
            var appName = Console.ReadLine();

            Console.Write("Enter device name: ");
            var deviceName = Console.ReadLine();

            var appKey = await ((HueClient)_hueClient).NewDeveloper(appName, deviceName);

            Console.WriteLine($"NewDeveloper app key (copy and save): {appKey}");
            Console.Write("Press any key to continue ...");
            Console.ReadKey();

            _settingsProvider.SetAppKey(appKey);
        }

        public async Task<SearchResult> GetNewLights()
        {
            var hueResults = await _hueClient.SearchNewLightsAsync();
            
            if (!ValidateHueResults(hueResults, out string[] errors))
            {
                return new SearchResult
                {
                    Success = false,
                    Errors = errors,
                    LightsFound = 0
                };
            }

            var newLights = await ScanForNewLights();

            if (newLights.Count < 3)
            {
                return new SearchResult
                {
                    Success = false,
                    Errors = new[] { $"{newLights.Count} lights found; minimum of 3 required to continue" },
                    LightsFound = newLights.Count
                };
            }

            return new SearchResult
            {
                Success = true,
                LightsFound = newLights.Count
            };
        }

        public async Task<SearchResult> SearchNewLights()
        {
            IList<string> serialNumbers = new List<string>();

            ConsoleKeyInfo enterSerialNumber;
            do
            {
                Console.Write("Enter serial number: ");
                var serialNumber = Console.ReadLine();

                serialNumbers.Add(serialNumber);

                Console.Write("Another serial number? (Y/N) ");
                enterSerialNumber = Console.ReadKey();
                Console.WriteLine();

            } while (enterSerialNumber.Key == ConsoleKey.Y);

            var hueResults = await _hueClient.SearchNewLightsAsync(serialNumbers);

            if (!ValidateHueResults(hueResults, out string[] errors))
            {
                return new SearchResult
                {
                    Success = false,
                    Errors = errors,
                    LightsFound = 0
                };
            }

            var newLights = await ScanForNewLights();

            if (newLights.Count < 3)
            {
                return new SearchResult
                {
                    Success = false,
                    Errors = new[] { $"{newLights.Count} found; minimum of 3 required to continue" },
                    LightsFound = newLights.Count
                };
            }

            return new SearchResult
            {
                Success = true,
                LightsFound = newLights.Count
            };
        }

        public async Task<bool> RunInitialSetup()
        {
            var success = true;

            foreach (var setupAction in _setupActions)
            {
                success = await setupAction.Execute();

                if (!success)
                    break;
            }

            return success;
        }

        private bool ValidateHueResults(HueResults hueResults, out string[] errors)
        {
            errors = hueResults.Errors.Select(error => error.Error.Description).ToArray();

            return !hueResults.HasErrors();
        }

        private async Task<IList<Light>> ScanForNewLights()
        {
            IList<Light> newLights;

            ConsoleKeyInfo? continueScanning;
            do
            {
                newLights = (await _hueClient.GetNewLightsAsync()).ToList();
                Thread.Sleep(2000);

                Console.Write($"{newLights.Count} new light(s) found. Continue scanning? (Y/N) ");
                continueScanning = Console.ReadKey();
                Console.WriteLine();

            } while (continueScanning.Value.Key == ConsoleKey.Y);

            return newLights;
        }
    }
}
