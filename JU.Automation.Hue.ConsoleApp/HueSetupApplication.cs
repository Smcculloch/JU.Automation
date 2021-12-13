using System;
using System.Threading;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;

namespace JU.Automation.Hue.ConsoleApp
{
    public class HueSetupApplication
    {
        private readonly IHueSetupCoordinator _hueSetupCoordinator;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ILogger<HueSetupApplication> _logger;

        public HueSetupApplication(
            IHueSetupCoordinator hueSetupCoordinator,
            ISettingsProvider settingsProvider,
            ILogger<HueSetupApplication> logger)
        {
            _hueSetupCoordinator = hueSetupCoordinator;
            _settingsProvider = settingsProvider;
            _logger = logger;
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            if (string.IsNullOrEmpty(_settingsProvider.LocalHueClientIp))
            {
                Console.WriteLine("No Hue Bridge IP!");
                Console.WriteLine($"Usage: {GetType().FullName}.exe --hue-ip <192.168.#.#> --hue-appkey <u2dKQBY1nIfGkB77HMNJnf7P4iie5rcEm4kXl1eD>");
                Console.WriteLine("--hue-ip mandatory");
                Console.WriteLine("--hue-apikey can be acquired by running \"New Developer\"");

                Environment.Exit(0);
            }

            var menuAction = RenderAndGetMenuSelection();

            while (menuAction != 0)
            {
                try
                {
                    switch (menuAction)
                    {
                        case -1:
                            Console.WriteLine("Invalid input");
                            break;
                        case 1:
                            Console.WriteLine($"Running show capabilities request{Environment.NewLine}");
                            await _hueSetupCoordinator.ShowCapabilities();
                            break;
                        case 2:
                            Console.WriteLine($"Running identify lights{Environment.NewLine}");
                            await _hueSetupCoordinator.IdentifyLightsAsync();
                            break;
                        case 3:
                            Console.WriteLine($"Running create new developer{Environment.NewLine}");
                            await _hueSetupCoordinator.CreateNewDeveloper();
                            break;
                        case 4:
                            Console.WriteLine($"Running setup{Environment.NewLine}");
                            await _hueSetupCoordinator.FullSetupAsync();
                            break;
                        case 5:
                            Console.WriteLine($"Running advanced setup{Environment.NewLine}");
                            await _hueSetupCoordinator.FullSetupAdvancedAsync();
                            break;
                        case 6:
                            Console.WriteLine($"Running reset{Environment.NewLine}");
                            await _hueSetupCoordinator.FullResetAsync();
                            break;
                        case 8:
                            Console.WriteLine($"Running automation(s) setup{Environment.NewLine}");
                            await _hueSetupCoordinator.CreateAutomationsAsync();
                            break;
                        case 9:
                            Console.WriteLine($"Running automation(s) reset{Environment.NewLine}");
                            await _hueSetupCoordinator.ResetAutomationsAsync();
                            break;
                        case 10:
                            Console.WriteLine($"Running sensor setup{Environment.NewLine}");
                            await _hueSetupCoordinator.SwitchSetupAsync();
                            break;
                        case 11:
                            Console.WriteLine($"Running switch reset{Environment.NewLine}");
                            await _hueSetupCoordinator.SwitchResetAsync();
                            break;
                        default:
                            Console.WriteLine("Invalid menu selection");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurred during setup");

                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine("Press any key to continue ...");
                Console.ReadKey();

                menuAction = RenderAndGetMenuSelection();
            }
        }

        private int RenderAndGetMenuSelection()
        {
            Console.Clear();

            Console.WriteLine($"Menu (HueIp: {_settingsProvider.LocalHueClientIp} AppKey: {_settingsProvider.AppKey})");
            Console.WriteLine($"{Environment.NewLine}--- Troubleshooting ---");
            Console.WriteLine(" (1) Show Capabilities");
            Console.WriteLine(" (2) Identify lights");
            Console.WriteLine($"{Environment.NewLine}--- Setup ---");
            Console.WriteLine(" (3) New Developer");
            Console.WriteLine(" (4) Full Setup");
            Console.WriteLine(" (5) Advanced Setup (using serial #'s)");
            Console.WriteLine(" (6) Full Reset");
            Console.WriteLine($"{Environment.NewLine}--- Automation ---");
            Console.WriteLine(" (8) Automation(s) Setup");
            Console.WriteLine(" (9) Automation(s) Reset");
            Console.WriteLine("(10) Sensor(s) Setup");
            Console.WriteLine("(11) Sensor(s) Reset");
            Console.WriteLine($"{Environment.NewLine}---");
            Console.WriteLine(" (0) Exit");
            Console.Write($"{Environment.NewLine}Select: ");

            var input = Console.ReadLine();

            Console.WriteLine(string.Empty);

            if (int.TryParse(input, out int result))
                return result;

            return -1;
        }
    }
}
