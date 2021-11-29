using System;
using System.Linq;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;

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
            var newSensors = await _hueClient.GetNewSensorsAsync();

            var sensor = newSensors.FirstOrDefault();

            await _hueClient.UpdateSensorAsync(sensor.Id, Constants.Switches.AllOff);

            Console.WriteLine($"Sensor ({sensor.Name}) name updated");

            model.TriggerSensor = sensor;

            return model;
        }
    }
}
