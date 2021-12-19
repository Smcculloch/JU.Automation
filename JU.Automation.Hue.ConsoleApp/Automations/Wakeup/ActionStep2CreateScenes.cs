using System;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Automations.Wakeup;

public class ActionStep2CreateScenes : ActionStepBase<ActionStep2CreateScenes, WakeupModel>
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

    public override async Task<WakeupModel> ExecuteStep(WakeupModel model)
    {
        if (model.Index == 0)
            throw new ArgumentException($"{nameof(model.Index)} must be greater than zero");

        if (model.RecurringDay == default)
            throw new ArgumentException($"{nameof(model.RecurringDay)} is invalid");

        if (model.WakeupTime == TimeSpan.Zero)
            throw new ArgumentException($"{nameof(model.WakeupTime)} is invalid");

        if (model.Group == null)
            throw new ArgumentNullException($"{model.Group} cannot be null");

        if (model.Lights == null)
            throw new ArgumentNullException($"{model.Lights} cannot be null");

        if (model.TriggerSensor == null)
            throw new ArgumentNullException($"{model.TriggerSensor} cannot be null");

        model.Scenes.Init = await CreateInitScene(model.Index, model.Group);
        model.Scenes.TransitionUp = await CreateTransitionUpScene(model.Index, model.Group);
        model.Scenes.TransitionDown = await CreateTransitionDownScene(model.Index, model.Group);
        model.Scenes.TurnOff = await CreateTurnOffScene(model.Index, model.Group);

        return model;
    }

    private async Task<Scene> CreateInitScene(int index, Group group)
    {
        var wakeupInitScene = new Scene
        {
            Name = $"{Constants.Automation.Wakeup}{index}{Constants.Entity.Scene}{Constants.Stage.Init}",
            Type = SceneType.GroupScene,
            Group = group.Id
        };

        var wakeupInitSceneId = await _hueClient.CreateSceneAsync(wakeupInitScene);

        foreach (var lightId in group.Lights)
        {
            await _hueClient.ModifySceneAsync(
                wakeupInitSceneId,
                lightId,
                new LightCommand
                {
                    On = true,
                    Brightness = 1,
                    ColorTemperature = 447,
                });
        }

        Console.WriteLine($"Scene ({wakeupInitScene.Name}) with id {wakeupInitSceneId} created");

        return await _hueClient.GetSceneAsync(wakeupInitSceneId);
    }

    private async Task<Scene> CreateTransitionUpScene(int index, Group group)
    {
        var wakeupTransitionUpScene = new Scene
        {
            Name = $"{Constants.Automation.Wakeup}{index}{Constants.Entity.Scene}{Constants.Stage.TransitionUp}",
            Type = SceneType.GroupScene,
            Group = group.Id
        };

        var wakeupTransitionUpSceneId = await _hueClient.CreateSceneAsync(wakeupTransitionUpScene);

        foreach (var lightId in group.Lights)
        {
            await _hueClient.ModifySceneAsync(
                wakeupTransitionUpSceneId,
                lightId,
                new LightCommand
                {
                    On = true,
                    Brightness = 255,
                    TransitionTime = TimeSpan.FromMinutes(_settingsProvider.WakeupTransitionUpInMinutes)
                                             .Subtract(TimeSpan.FromSeconds(Constants.ScheduleDeactivateDelayInSeconds))
                });
        }

        Console.WriteLine($"Scene ({wakeupTransitionUpScene.Name}) with id {wakeupTransitionUpSceneId} created");

        return await _hueClient.GetSceneAsync(wakeupTransitionUpSceneId);
    }

    private async Task<Scene> CreateTransitionDownScene(int index, Group group)
    {
        var wakeupTransitionDownScene = new Scene
        {
            Name = $"{Constants.Automation.Wakeup}{index}{Constants.Entity.Scene}{Constants.Stage.TransitionDown}",
            Type = SceneType.GroupScene,
            Group = group.Id
        };

        var wakeupTransitionDownSceneId = await _hueClient.CreateSceneAsync(wakeupTransitionDownScene);

        foreach (var lightId in group.Lights)
        {
            await _hueClient.ModifySceneAsync(
                wakeupTransitionDownSceneId,
                lightId,
                new LightCommand
                {
                    On = true,
                    Brightness = 1,
                    ColorTemperature = 447,
                    TransitionTime = TimeSpan.FromMinutes(_settingsProvider.WakeupTransitionDownInMinutes)
                                             .Subtract(TimeSpan.FromSeconds(Constants
                                                 .ScheduleDeactivateDelayInSeconds)),
                });
        }

        Console.WriteLine($"Scene ({wakeupTransitionDownScene.Name}) with id {wakeupTransitionDownSceneId} created");

        return await _hueClient.GetSceneAsync(wakeupTransitionDownSceneId);
    }

    private async Task<Scene> CreateTurnOffScene(int index, Group group)
    {
        var wakeupTurnOffScene = new Scene
        {
            Name = $"{Constants.Automation.Wakeup}{index}{Constants.Entity.Scene}{Constants.Stage.TurnOff}",
            Type = SceneType.GroupScene,
            Group = group.Id
        };

        var wakeupTurnOffSceneId = await _hueClient.CreateSceneAsync(wakeupTurnOffScene);

        foreach (var lightId in group.Lights)
        {
            await _hueClient.ModifySceneAsync(
                wakeupTurnOffSceneId,
                lightId,
                new LightCommand
                {
                    On = false,
                    Brightness = 0
                });
        }

        Console.WriteLine($"Scene ({wakeupTurnOffScene.Name}) with id {wakeupTurnOffSceneId} created");

        return await _hueClient.GetSceneAsync(wakeupTurnOffSceneId);
    }
}
