using System;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Sensors.CLIP;

namespace JU.Automation.Hue.ConsoleApp.Automations.Wakeup
{
    public class AutomationSetupActionStep1CreateSensors : AutomationSetupActionStepBase<AutomationSetupActionStep1CreateSensors, WakeupModel>
    {
        private readonly IHueClient _hueClient;
        private readonly ISettingsProvider _settingsProvider;

        public AutomationSetupActionStep1CreateSensors(
            IHueClient hueClient,
            ILogger<AutomationSetupActionStep1CreateSensors> logger,
            ISettingsProvider settingsProvider): base(logger)
        {
            _hueClient = hueClient;
            _settingsProvider = settingsProvider;
        }

        public override int Step => 1;

        public override async Task<WakeupModel> ExecuteStep(WakeupModel model)
        {
            if (model.Group == null)
                throw new ArgumentNullException($"{nameof(model.Group)} cannot be null");

            if (model.Lights == null)
                throw new ArgumentNullException($"{nameof(model.Lights)} cannot be null");

            var wakeup1Sensor = new Sensor
            {
                Config = new SensorConfig
                {
                    On = true,
                    Reachable = true
                },
                Name = "Wake-up 1",
                Type = nameof(CLIPGenericFlag),
                ModelId = "WAKEUP",
                ManufacturerName = "Philips",
                SwVersion = "1.0",
                UniqueId = _settingsProvider.Wakeup1SensorUniqueId
            };

            var sensorId = await _hueClient.CreateSensorAsync(wakeup1Sensor);

            Console.WriteLine($"Sensor ({wakeup1Sensor.Name}) with id {sensorId} created");

            return new WakeupModel
            {
                Group = model.Group,
                Lights = model.Lights,
                TriggerSensor = await _hueClient.GetSensorAsync(sensorId)
            };
        }
    }
}
