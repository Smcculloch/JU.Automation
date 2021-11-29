using System;
using System.Threading;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Providers;

namespace JU.Automation.Hue.ConsoleApp
{
    public class HueSetupApplication
    {
        private readonly IHueSetupCoordinator _hueSetupCoordinator;
        private readonly ISettingsProvider _settingsProvider;

        public HueSetupApplication(
            IHueSetupCoordinator hueSetupCoordinator,
            ISettingsProvider settingsProvider)
        {
            _hueSetupCoordinator = hueSetupCoordinator;
            _settingsProvider = settingsProvider;
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            var menuAction = RenderAndGetMenuSelection();

            while (menuAction != 0)
            {
                switch (menuAction)
                {
                    case -1:
                        Console.WriteLine("Invalid input");
                        break;
                    case 1:
                        Console.WriteLine($"Running config request{Environment.NewLine}");
                        await _hueSetupCoordinator.GetConfig();
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
                        await _hueSetupCoordinator.SensorSetupAsync();
                        break;
                    default:
                        Console.WriteLine("Invalid menu selection");
                        break;
                }

                Console.WriteLine("Press any key to continue ...");
                Console.ReadKey();

                menuAction = RenderAndGetMenuSelection();
            }

            Environment.Exit(0);
        }

        private int RenderAndGetMenuSelection()
        {
            Console.Clear();

            Console.WriteLine($"Menu (Hue: {_settingsProvider.LocalHueClientIp})");
            Console.WriteLine($"{Environment.NewLine}--- Troubleshooting ---");
            Console.WriteLine(" (1) Show Config");
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
