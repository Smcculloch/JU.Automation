using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Actions.Initial
{
    public class SetupActionStep2GroupLights : SetupActionStepBase<SetupActionStep2GroupLights>
    {
        private readonly IHueClient _hueClient;
        private readonly ISettingsProvider _settingsProvider;

        public SetupActionStep2GroupLights(
            IHueClient hueClient,
            ILogger<SetupActionStep2GroupLights> logger,
            ISettingsProvider settingsProvider) : base(logger)
        {
            _hueClient = hueClient;
            _settingsProvider = settingsProvider;
        }

        public override int Step => 2;

        public override async Task<bool> ExecuteStep()
        {
            var newLights = (await _hueClient.GetNewLightsAsync()).AsEnumerable();

            Console.WriteLine($"Setup {Constants.Groups.Bedroom}");
            Console.WriteLine("Select lights:");
            var groupLights = await SelectGroupLights(newLights);
            await _hueClient.CreateGroupAsync(groupLights.Select(light => light.Id), Constants.Groups.Bedroom, RoomClass.Bedroom);
            Console.WriteLine();

            newLights = newLights.Except(groupLights);

            Console.WriteLine($"Setup {Constants.Groups.Kitchen}");
            Console.WriteLine("Select lights:");
            groupLights = await SelectGroupLights(newLights);
            await _hueClient.CreateGroupAsync(groupLights.Select(light => light.Id), Constants.Groups.Kitchen, RoomClass.Kitchen);
            Console.WriteLine();

            newLights = newLights.Except(groupLights);

            Console.WriteLine($"Setup {Constants.Groups.LivingRoom}");
            Console.WriteLine($"Using remaining light(s) in the living room: {string.Join(", ", newLights.Select(light => light.Id))}");
            await _hueClient.CreateGroupAsync(newLights.Select(light => light.Id), Constants.Groups.LivingRoom, RoomClass.LivingRoom);
            Console.WriteLine();

            return true;
        }

        private async Task<IEnumerable<Light>> SelectGroupLights(IEnumerable<Light> newLights)
        {
            var lights = newLights.ToDictionary(light => light.Id);

            var groupLights = new List<Light>();
            var keepScanning = new ConsoleKeyInfo('y', ConsoleKey.Y, true, false, false);

            do
            {
                foreach (var light in newLights.Except(groupLights))
                {
                    Console.WriteLine($"({light.Id}) {light.Name}");
                }

                Console.Write("Select light number (#): ");
                var lightId = Console.ReadLine();

                if (!lights.ContainsKey(lightId))
                {
                    Console.WriteLine("Invalid input");
                    continue;
                }

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
