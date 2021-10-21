using System;
using System.Threading;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Actions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Hosting;

namespace JU.Automation.Hue.ConsoleApp
{
    public class HueSetupApplication//: BackgroundService
    {
        private readonly IHueConfigService _hueConfigService;
        private readonly ISettingsProvider _settingsProvider;

        public HueSetupApplication(
            IHueConfigService hueConfigService,
            ISettingsProvider settingsProvider)
        {
            _hueConfigService = hueConfigService;
            _settingsProvider = settingsProvider;
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
                        Console.WriteLine($"Running setup{Environment.NewLine}");
                        await _hueConfigService.FullSetupAsync();
                        break;
                    case 2:
                        Console.WriteLine($"Running automations setup{Environment.NewLine}");
                        await _hueConfigService.CreateAutomationsAsync();
                        break;
                    case 8:
                        Console.WriteLine($"Running reset{Environment.NewLine}");
                        await _hueConfigService.FullResetAsync();
                        break;
                    case 9:
                        Console.WriteLine($"Running automations reset{Environment.NewLine}");
                        await _hueConfigService.ResetAutomationsAsync();
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
            Console.WriteLine("(1) Full Setup");
            Console.WriteLine("(2) Automations Setup");
            Console.WriteLine("(8) Full Reset");
            Console.WriteLine("(9) Automations Reset");
            Console.WriteLine("(0) Exit");
            Console.Write("Select: ");

            var input = Console.ReadLine();

            Console.WriteLine(string.Empty);

            if (int.TryParse(input, out int result))
                return result;

            return -1;
        }
    }
}
