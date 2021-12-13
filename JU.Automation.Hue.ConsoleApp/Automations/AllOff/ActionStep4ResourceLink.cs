using System;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.AllOff
{
    public class ActionStep4ResourceLink : ActionStepBase<ActionStep4ResourceLink, SwitchModel>
    {
        private readonly IHueClient _hueClient;
        private readonly ISettingsProvider _settingsProvider;

        public ActionStep4ResourceLink(
            IHueClient hueClient,
            ILogger<ActionStep4ResourceLink> logger,
            ISettingsProvider settingsProvider): base(logger)
        {
            _hueClient = hueClient;
            _settingsProvider = settingsProvider;
        }

        public override int Step => 5;

        public override async Task<SwitchModel> ExecuteStep(SwitchModel model)
        {
            if (model.TriggerSensor == null)
                throw new ArgumentNullException($"{nameof(model.TriggerSensor)} cannot be null");

            if (model.VirtualSensors?.Wakeup == null || model.VirtualSensors?.Sunrise == null || model.VirtualSensors?.Bedtime == null)
                throw new ArgumentNullException($"One or more virtual sensors are null");

            if (model.TriggerSensor == null)
                throw new ArgumentNullException($"{model.TriggerSensor} cannot be null");

            if (model.Scenes?.AllOff == null)
                throw new ArgumentNullException($"${nameof(model.Scenes.AllOff)} scene is null");

            if (model.Rules?.AllOff == null)
                throw new ArgumentNullException($"${nameof(model.Rules.AllOff)} rule is null");

            await CreateResourceLink(model.TriggerSensor, model.VirtualSensors, model.Scenes, model.Rules);

            return model;
        }

        private async Task<ResourceLink> CreateResourceLink(Sensor sensor, VirtualSensors virtualSensors, SwitchScenes scenes, SwitchRules rules)
        {
            var resourceLink = new ResourceLink
            {
                Name = "AllOff",
                Description = "JU AllOff Switch",
                ClassId = 2,
                Links =
                {
                    $"/sensors/{sensor.Id}",
                    $"/sensors/{virtualSensors.Wakeup.Id}",
                    $"/sensors/{virtualSensors.Sunrise.Id}",
                    $"/sensors/{virtualSensors.Bedtime.Id}",
                    $"/{nameof(scenes)}/{scenes.AllOff.Id}",
                    $"/{nameof(rules)}/{rules.AllOff.Id}"
                }
            };

            var resourceLinkId = await _hueClient.CreateResourceLinkAsync(resourceLink);

            Console.WriteLine($"ResourceLink {resourceLink.Name} with id {resourceLinkId} created");

            resourceLink.Id = resourceLinkId;

            return resourceLink;
        }
    }
}
