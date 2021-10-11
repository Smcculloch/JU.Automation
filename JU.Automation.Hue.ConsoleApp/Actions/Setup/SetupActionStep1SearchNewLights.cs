using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Actions.Setup
{
    public class SetupActionStep1SearchNewLights : SetupActionStepBase<SetupActionStep1SearchNewLights>
    {
        private readonly IHueClient _hueClient;

        public SetupActionStep1SearchNewLights(
            IHueClient hueClient,
            ILogger<SetupActionStep1SearchNewLights> logger): base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 1;

        public override async Task ExecuteStep()
        {
            var hueResults = await _hueClient.SearchNewLightsAsync();

            if (hueResults.HasErrors())
            {
                var errors = string.Join(Environment.NewLine, hueResults.Errors.Select(error => error.Error.Description));
                Console.WriteLine($"Search error(s):{Environment.NewLine}{errors}");
                return;
            }

            ConsoleKeyInfo continueScanning;

            do
            {
                var newLights = await _hueClient.GetNewLightsAsync();

                Thread.Sleep(500);

                Console.Write($"{newLights.Count} new light(s) found. Continue scanning? (Y/N) ");
                continueScanning = Console.ReadKey();
                Console.WriteLine();

            } while (continueScanning.Key == ConsoleKey.Y);
        }
    }
}
