using System;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Actions.Initial
{
    public class SetupActionStep1RenameLights : SetupActionStepBase<SetupActionStep1RenameLights>
    {
        private readonly IHueClient _hueClient;
        private readonly ISettingsProvider _settingsProvider;

        public SetupActionStep1RenameLights(
            IHueClient hueClient,
            ILogger<SetupActionStep1RenameLights> logger,
            ISettingsProvider settingsProvider) : base(logger)
        {
            _hueClient = hueClient;
            _settingsProvider = settingsProvider;
        }

        public override int Step => 1;

        public override async Task<bool> ExecuteStep()
        {
            var newLights = await _hueClient.GetNewLightsAsync();

            foreach (var light in newLights)
            {
                await _hueClient.SendCommandAsync(new LightCommand { Alert = Alert.Multiple }, new[] { light.Id });

                Console.Write($"Enter light {light.Name} ({light.Id}) name: ");
                var lightName = Console.ReadLine();

                await _hueClient.SendCommandAsync(new LightCommand { Alert = Alert.None }, new[] { light.Id });
                await _hueClient.SetLightNameAsync(light.Id, lightName);
            }

            Console.WriteLine();

            return true;
        }
    }
}
