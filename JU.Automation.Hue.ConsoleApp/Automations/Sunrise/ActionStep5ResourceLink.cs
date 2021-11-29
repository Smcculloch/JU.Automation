using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.Sunrise
{
    public class ActionStep5ResourceLink : ActionStepBase<ActionStep5ResourceLink, SunriseModel>
    {
        private readonly IHueClient _hueClient;

        public ActionStep5ResourceLink(
            IHueClient hueClient,
            ILogger<ActionStep5ResourceLink> logger): base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 5;

        public override async Task<SunriseModel> ExecuteStep(SunriseModel model)
        {
            if (model.WakeupTime == TimeSpan.Zero)
                throw new ArgumentException($"{nameof(model.WakeupTime)} is invalid");

            if (model.DepartureTime == TimeSpan.Zero)
                throw new ArgumentException($"{nameof(model.DepartureTime)} is invalid");

            if (model.Group == null)
                throw new ArgumentNullException($"{nameof(model.Group)} cannot be null");

            if (model.Lights == null)
                throw new ArgumentNullException($"{nameof(model.Lights)} cannot be null");

            if (model.TriggerSensor == null)
                throw new ArgumentNullException($"{nameof(model.TriggerSensor)} cannot be null");

            if (model.Scenes?.Init == null || model.Scenes?.TransitionUp == null || model.Scenes?.TurnOff == null)
                throw new ArgumentNullException($"One or more scenes are null");

            if (model.Schedules?.Start == null || model.Schedules?.TransitionUp == null || model.Schedules?.TurnOff == null)
                throw new ArgumentNullException($"One or more schedules are null");

            if (model.Rules?.Trigger == null || model.Rules?.TurnOff == null)
                throw new ArgumentNullException($"One or more rules are null");

            await CreateResourceLink(model.TriggerSensor, model.Scenes, model.Schedules, model.Rules);

            return model;
        }

        private async Task<ResourceLink> CreateResourceLink(Sensor sensor, SunriseScenes scenes, SunriseSchedules schedules, SunriseRules rules)
        {
            var resourceLink = new ResourceLink
            {
                Name = "Sunrise",
                Description = "JU Sunrise Automation",
                ClassId = 10,
                Links =
                {
                    $"/sensors/{sensor.Id}",
                    $"/{nameof(scenes)}/{scenes.Init.Id}",
                    $"/{nameof(scenes)}/{scenes.TransitionUp.Id}",
                    $"/{nameof(scenes)}/{scenes.TurnOff.Id}",
                    $"/{nameof(schedules)}/{schedules.Start.Id}",
                    $"/{nameof(schedules)}/{schedules.TransitionUp.Id}",
                    $"/{nameof(schedules)}/{schedules.TurnOff.Id}",
                    $"/{nameof(rules)}/{rules.Trigger.Id}",
                    $"/{nameof(rules)}/{rules.TurnOff.Id}"
                }
            };

            var resourceLinkId = await _hueClient.CreateResourceLinkAsync(resourceLink);

            Console.WriteLine($"ResourceLink {resourceLink.Name} with id {resourceLinkId} created");

            resourceLink.Id = resourceLinkId;

            return resourceLink;
        }
    }
}
