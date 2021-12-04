using System;
using System.Linq;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.AllOff
{
    public class ActionStep1RenameSensor : ActionStepBase<ActionStep1RenameSensor, SwitchModel>
    {
        private readonly IHueClient _hueClient;

        public ActionStep1RenameSensor(
            IHueClient hueClient,
            ILogger<ActionStep1RenameSensor> logger): base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 1;

        public override async Task<SwitchModel> ExecuteStep(SwitchModel model)
        {
            if (model.Lights == null)
                throw new ArgumentNullException($"{model.Lights} cannot be null");

            if (model.VirtualSensors?.Wakeup == null || model.VirtualSensors?.Sunrise == null || model.VirtualSensors?.Bedtime == null)
                throw new ArgumentNullException($"One or more virtual sensors are null");

            var newSensors = await _hueClient.GetNewSensorsAsync();

            Sensor allOffSensor;

            if (newSensors.Count == 1)
            {
                allOffSensor = newSensors.FirstOrDefault();

                await _hueClient.UpdateSensorAsync(allOffSensor.Id, Constants.Switches.AllOff);

                allOffSensor = await _hueClient.GetSensorAsync(allOffSensor?.Id);

                Console.WriteLine($"Sensor ({allOffSensor.Name}) name updated");
            }
            else
            {
                var allSensors = await _hueClient.GetSensorsAsync();

                allOffSensor = allSensors.FirstOrDefault(sensor => sensor.Name == Constants.Switches.AllOff);
            }

            model.TriggerSensor = allOffSensor;

            return model;
        }
    }
}
