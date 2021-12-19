using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.AllOff;

public class ActionStep2CreateScenes : ActionStepBase<ActionStep2CreateScenes, SwitchModel>
{
    private readonly IHueClient _hueClient;
    private readonly ISettingsProvider _settingsProvider;

    public ActionStep2CreateScenes(
        IHueClient hueClient,
        ILogger<ActionStep2CreateScenes> logger,
        ISettingsProvider settingsProvider) : base(logger)
    {
        _hueClient = hueClient;
        _settingsProvider = settingsProvider;
    }

    public override int Step => 2;

    public override async Task<SwitchModel> ExecuteStep(SwitchModel model)
    {
        if (model.Lights == null)
            throw new ArgumentNullException($"{model.Lights} cannot be null");

        if (model.Sensors?.Wakeup == null || model.Sensors?.Sunrise == null || model.Sensors?.Bedtime == null)
            throw new ArgumentNullException($"One or more virtual sensors are null");

        if (model.TriggerSensor == null)
            throw new ArgumentNullException($"{model.TriggerSensor} cannot be null");

        model.Scenes.AllOff = await CreateAllOffScene(model.Lights);

        return model;
    }

    private async Task<Scene> CreateAllOffScene(IList<Light> lights)
    {
        var allOffScene = new Scene
        {
            Name = Constants.Scenes.AllOff,
            Type = SceneType.LightScene,
            Lights = lights.Select(light => light.Id),
            Recycle = true
        };

        var allOffSceneId = await _hueClient.CreateSceneAsync(allOffScene);

        foreach (var lightId in allOffScene.Lights)
        {
            await _hueClient.ModifySceneAsync(
                allOffSceneId,
                lightId,
                new LightCommand
                {
                    On = false,
                    Brightness = 1,
                    ColorTemperature = 447,
                });
        }

        allOffScene = await _hueClient.GetSceneAsync(allOffSceneId);

        Console.WriteLine($"Scene ({allOffScene.Name}) with id {allOffSceneId} created");

        return allOffScene;
    }
}
