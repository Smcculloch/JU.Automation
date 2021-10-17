﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Actions.Setup
{
    public class SetupActionStep3RenameLights : SetupActionStepBase<SetupActionStep3RenameLights>
    {
        private readonly IHueClient _hueClient;

        public SetupActionStep3RenameLights(
            IHueClient hueClient,
            ILogger<SetupActionStep3RenameLights> logger): base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 3;

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