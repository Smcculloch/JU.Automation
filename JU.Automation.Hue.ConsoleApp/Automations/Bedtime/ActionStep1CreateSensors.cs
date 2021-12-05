using System;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Sensors.CLIP;

namespace JU.Automation.Hue.ConsoleApp.Automations.Bedtime
{
    public class ActionStep1CreateSensors : ActionStepBase<ActionStep1CreateSensors, BedtimeModel>
    {
        private readonly IHueClient _hueClient;
        private readonly ISettingsProvider _settingsProvider;

        public ActionStep1CreateSensors(
            IHueClient hueClient,
            ILogger<ActionStep1CreateSensors> logger,
            ISettingsProvider settingsProvider): base(logger)
        {
            _hueClient = hueClient;
            _settingsProvider = settingsProvider;
        }

        public override int Step => 1;

        public override async Task<BedtimeModel> ExecuteStep(BedtimeModel model)
        {
            if (model.RecurringDay == default)
                throw new ArgumentException($"{nameof(model.RecurringDay)} is invalid");

            if (model.BedtimeTime == TimeSpan.Zero)
                throw new ArgumentException($"{nameof(model.BedtimeTime)} is invalid");

            if (model.Group == null)
                throw new ArgumentNullException($"{nameof(model.Group)} cannot be null");

            if (model.Lights == null)
                throw new ArgumentNullException($"{nameof(model.Lights)} cannot be null");

            var bedtimeSensor = new Sensor
            {
                Config = new SensorConfig
                {
                    On = true,
                    Reachable = true
                },
                Name = Constants.VirtualSensors.Bedtime,
                Type = nameof(CLIPGenericFlag),
                ModelId = "BEDTIME",
                ManufacturerName = "Philips",
                SwVersion = "1.0",
                UniqueId = $"{Guid.NewGuid():N}"
            };

            var sensorId = await _hueClient.CreateSensorAsync(bedtimeSensor);

            Console.WriteLine($"Sensor ({bedtimeSensor.Name}) with id {sensorId} created");

            model.TriggerSensor = await _hueClient.GetSensorAsync(sensorId);

            return model;
        }
    }
}
