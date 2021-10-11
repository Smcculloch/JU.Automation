using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Actions.Setup
{
    public class SetupActionStep3GroupLights : SetupActionStepBase<SetupActionStep3GroupLights>
    {
        private readonly IHueClient _hueClient;

        public SetupActionStep3GroupLights(
            IHueClient hueClient,
            ILogger<SetupActionStep3GroupLights> logger): base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 3;

        public override async Task ExecuteStep()
        {
            var newLights = (await _hueClient.GetNewLightsAsync()).AsEnumerable();

            Console.WriteLine("Setup Living Room");
            Console.WriteLine("Select lights:");
            var groupLights = await SelectGroupLights(newLights);
            await _hueClient.CreateGroupAsync(groupLights.Select(light => light.Id), "Living Room", RoomClass.LivingRoom);
            Console.WriteLine();

            newLights = newLights.Except(groupLights);

            Console.WriteLine("Setup Bedroom");
            Console.WriteLine("Select lights:");
            groupLights = await SelectGroupLights(newLights);
            await _hueClient.CreateGroupAsync(groupLights.Select(light => light.Id), "Bedroom", RoomClass.Bedroom);
            Console.WriteLine();
        }

        private async Task<IEnumerable<Light>> SelectGroupLights(IEnumerable<Light> newLights)
        {
            var lights = newLights.ToDictionary(light => light.Id);

            var groupLights = new List<Light>();
            ConsoleKeyInfo keepScanning;

            do
            {
                foreach (var light in newLights.Except(groupLights))
                {
                    Console.WriteLine($"({light.Id}) {light.Name}");
                }

                Console.Write("Select light number (#): ");
                var lightId = Console.ReadLine();

                if (!lights.ContainsKey(lightId))
                    Console.WriteLine("Invalid input");
                else
                    groupLights.Add(lights[lightId]);

                await _hueClient.SendCommandAsync(new LightCommand { Alert = Alert.Multiple }, new[] { lightId });

                Console.Write($"{string.Join(", ", groupLights.Select(light => $"({light.Id}) {light.Name}"))} selected. Add more lights? (Y/N) ");
                keepScanning = Console.ReadKey();
                Console.WriteLine();

            } while (keepScanning.Key == ConsoleKey.Y);

            foreach (var light in groupLights)
                await _hueClient.SendCommandAsync(new LightCommand { Alert = Alert.None }, new[] { light.Id });

            return groupLights;
        }
    }
}
