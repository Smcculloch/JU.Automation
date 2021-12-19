using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.AllOff;

public class ActionStep4ResourceLink : ActionStepBase<ActionStep4ResourceLink, SwitchModel>
{
    private readonly IHueClient _hueClient;
    private readonly ISettingsProvider _settingsProvider;

    public ActionStep4ResourceLink(
        IHueClient hueClient,
        ILogger<ActionStep4ResourceLink> logger,
        ISettingsProvider settingsProvider) : base(logger)
    {
        _hueClient = hueClient;
        _settingsProvider = settingsProvider;
    }

    public override int Step => 5;

    public override async Task<SwitchModel> ExecuteStep(SwitchModel model)
    {
        if (model.TriggerSensor == null)
            throw new ArgumentNullException($"{nameof(model.TriggerSensor)} cannot be null");

        if (model.Sensors?.Wakeup == null || model.Sensors?.Sunrise == null || model.Sensors?.Bedtime == null)
            throw new ArgumentNullException($"One or more virtual sensors are null");

        if (model.TriggerSensor == null)
            throw new ArgumentNullException($"{model.TriggerSensor} cannot be null");

        if (model.Scenes?.AllOff == null)
            throw new ArgumentNullException($"${nameof(model.Scenes.AllOff)} scene is null");

        if (model.Rules?.AllOff == null)
            throw new ArgumentNullException($"${nameof(model.Rules.AllOff)} rule is null");

        await CreateResourceLink(model.TriggerSensor, model.Sensors, model.Scenes, model.Rules);

        return model;
    }

    private async Task<ResourceLink> CreateResourceLink(Sensor sensor, VirtualSensors virtualSensors,
        SwitchScenes scenes, SwitchRules rules)
    {
        var resourceLink = new ResourceLink
        {
            Name = "AllOff",
            Description = "JU AllOff Switch",
            ClassId = 2,
            Links = new List<string>()
        };

        resourceLink.Links.Add($"/sensors/{sensor.Id}");
        resourceLink.Links.AddRange(virtualSensors.Wakeup.Select(sensor => $"/sensors/{sensor.Id}"));
        resourceLink.Links.AddRange(virtualSensors.Sunrise.Select(sensor => $"/sensors/{sensor.Id}"));
        resourceLink.Links.Add($"/sensors/{virtualSensors.Bedtime.Id}");
        resourceLink.Links.Add($"/{nameof(scenes)}/{scenes.AllOff.Id}");
        resourceLink.Links.Add($"/{nameof(rules)}/{rules.AllOff.Id}");

        var resourceLinkId = await _hueClient.CreateResourceLinkAsync(resourceLink);

        Console.WriteLine($"ResourceLink {resourceLink.Name} with id {resourceLinkId} created");

        resourceLink.Id = resourceLinkId;

        return resourceLink;
    }
}
