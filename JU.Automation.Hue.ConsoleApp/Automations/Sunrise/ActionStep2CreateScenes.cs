using System;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Automations.Sunrise;

public class ActionStep2CreateScenes : ActionStepBase<ActionStep2CreateScenes, SunriseModel>
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

    public override async Task<SunriseModel> ExecuteStep(SunriseModel model)
    {
        if (model.Index == 0)
            throw new ArgumentException($"{nameof(model.Index)} must be greater than zero");

        if (model.RecurringDay == default)
            throw new ArgumentException($"{nameof(model.RecurringDay)} is invalid");

        if (model.WakeupTime == TimeSpan.Zero)
            throw new ArgumentException($"{nameof(model.WakeupTime)} is invalid");

        if (model.DepartureTime == TimeSpan.Zero)
            throw new ArgumentException($"{nameof(model.DepartureTime)} is invalid");

        if (model.Group == null)
            throw new ArgumentNullException($"{model.Group} cannot be null");

        if (model.Lights == null)
            throw new ArgumentNullException($"{model.Lights} cannot be null");

        if (model.TriggerSensor == null)
            throw new ArgumentNullException($"{model.TriggerSensor} cannot be null");

        model.Scenes.Init = await CreateInitScene(model.Index, model.Group);
        model.Scenes.TransitionUp = await CreateTransitionUpScene(model.Index, model.Group);
        model.Scenes.TurnOff = await CreateTurnOffScene(model.Index, model.Group);

        return model;
    }

    private async Task<Scene> CreateInitScene(int index, Group group)
    {
        var sunriseInitScene = new Scene
        {
            Name = $"{Constants.Automation.Sunrise}{index}{Constants.Entity.Scene}{Constants.Stage.Init}",
            Type = SceneType.GroupScene,
            Group = group.Id
        };

        var sunriseInitSceneId = await _hueClient.CreateSceneAsync(sunriseInitScene);

        foreach (var lightId in group.Lights)
        {
            await _hueClient.ModifySceneAsync(
                sunriseInitSceneId,
                lightId,
                new LightCommand
                {
                    On = true,
                    Brightness = 1,
                    ColorTemperature = 447,
                });
        }

        Console.WriteLine($"Scene ({sunriseInitScene.Name}) with id {sunriseInitSceneId} created");

        return await _hueClient.GetSceneAsync(sunriseInitSceneId);
    }

    private async Task<Scene> CreateTransitionUpScene(int index, Group group)
    {
        var sunriseTransitionUpScene = new Scene
        {
            Name = $"{Constants.Automation.Sunrise}{index}{Constants.Entity.Scene}{Constants.Stage.TransitionUp}",
            Type = SceneType.GroupScene,
            Group = group.Id
        };

        var sunriseTransitionUpSceneId = await _hueClient.CreateSceneAsync(sunriseTransitionUpScene);

        foreach (var lightId in group.Lights)
        {
            await _hueClient.ModifySceneAsync(
                sunriseTransitionUpSceneId,
                lightId,
                new LightCommand
                {
                    On = true,
                    Brightness = 255,
                    TransitionTime = TimeSpan.FromMinutes(_settingsProvider.SunriseTransitionUpInMinutes)
                                             .Subtract(TimeSpan.FromSeconds(Constants.ScheduleDeactivateDelayInSeconds))
                });
        }

        Console.WriteLine($"Scene ({sunriseTransitionUpScene.Name}) with id {sunriseTransitionUpSceneId} created");

        return await _hueClient.GetSceneAsync(sunriseTransitionUpSceneId);
    }

    private async Task<Scene> CreateTurnOffScene(int index, Group group)
    {
        var sunriseTurnOffScene = new Scene
        {
            Name = $"{Constants.Automation.Sunrise}{index}{Constants.Entity.Scene}{Constants.Stage.TurnOff}",
            Type = SceneType.GroupScene,
            Group = group.Id
        };

        var sunriseTurnOffSceneId = await _hueClient.CreateSceneAsync(sunriseTurnOffScene);

        foreach (var lightId in group.Lights)
        {
            await _hueClient.ModifySceneAsync(
                sunriseTurnOffSceneId,
                lightId,
                new LightCommand
                {
                    On = false,
                    Brightness = 0
                });
        }

        Console.WriteLine($"Scene ({sunriseTurnOffScene.Name}) with id {sunriseTurnOffSceneId} created");

        return await _hueClient.GetSceneAsync(sunriseTurnOffSceneId);
    }
}
