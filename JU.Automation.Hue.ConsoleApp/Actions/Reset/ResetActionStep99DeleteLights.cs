using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Actions.Reset
{
    public class ResetActionStep99DeleteLights : ResetActionStepBase<ResetActionStep99DeleteLights>
    {
        private readonly IHueClient _hueClient;

        public ResetActionStep99DeleteLights(
            IHueClient hueClient,
            ILogger<ResetActionStep99DeleteLights> logger) : base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 99;

        public override async Task ExecuteStep()
        {
            var lights = await _hueClient.GetLightsAsync();

            foreach (var light in lights)
            {
                await _hueClient.DeleteLightAsync(light.Id);
            }

            Console.WriteLine($"Deleted {lights.Count()} lights");
        }
    }
}
