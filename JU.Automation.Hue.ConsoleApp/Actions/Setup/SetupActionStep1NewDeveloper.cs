using System;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Actions.Setup
{
    public class SetupActionStep1NewDeveloper : SetupActionStepBase<SetupActionStep1NewDeveloper>
    {
        private readonly IHueClient _hueClient;
        private readonly ISettingsProvider _settingsProvider;

        public SetupActionStep1NewDeveloper(
            IHueClient hueClient,
            ISettingsProvider settingsProvider,
            ILogger<SetupActionStep1NewDeveloper> logger): base(logger)
        {
            _hueClient = hueClient;
            _settingsProvider = settingsProvider;
        }

        public override int Step => 1;

        public override async Task ExecuteStep()
        {
            if (!string.IsNullOrEmpty(_settingsProvider.AppKey))
            {
                Console.WriteLine("Skipping new developer step");
                return;
            }

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
    }
}
