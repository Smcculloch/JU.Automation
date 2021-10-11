using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Actions.AutomationReset
{
    public class AutomationResetActionStep99DeleteSensors : AutomationResetActionStepBase<AutomationResetActionStep99DeleteSensors>
    {
        private readonly IHueClient _hueClient;

        public AutomationResetActionStep99DeleteSensors(
            IHueClient hueClient,
            ILogger<AutomationResetActionStep99DeleteSensors> logger) : base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 99;

        public override async Task ExecuteStep()
        {
            var sensors = await _hueClient.GetSensorsAsync();

            foreach (var sensor in sensors)
            {
                await _hueClient.DeleteSensorAsync(sensor.Id);
            }

            Console.WriteLine($"Deleted {sensors.Count} sensors");
        }
    }
}
