using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Actions.Setup
{
    public class SetupActionStep2RenameLights : SetupActionStepBase<SetupActionStep2RenameLights>
    {
        private readonly IHueClient _hueClient;

        public SetupActionStep2RenameLights(
            IHueClient hueClient,
            ILogger<SetupActionStep2RenameLights> logger): base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 2;

        public override async Task ExecuteStep()
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
        }
    }
}
