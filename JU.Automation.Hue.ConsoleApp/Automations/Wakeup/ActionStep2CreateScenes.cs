using System;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Automations.Wakeup
{
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
            if (model.WakeupTime == TimeSpan.Zero)
                throw new ArgumentException($"{nameof(model.WakeupTime)} is invalid");

            if (model.Group == null)
                throw new ArgumentNullException($"{model.Group} cannot be null");

            if (model.Lights == null)
                throw new ArgumentNullException($"{model.Lights} cannot be null");

            if (model.TriggerSensor == null)
                throw new ArgumentNullException($"{model.TriggerSensor} cannot be null");

            model.Scenes.Init = await CreateInitScene(model.Group);
            model.Scenes.TransitionUp = await CreateTransitionUpScene(model.Group);
            model.Scenes.TransitionDown = await CreateTransitionDownScene(model.Group);
            model.Scenes.TurnOff = await CreateTurnOffScene(model.Group);

            return model;
        }

        private async Task<Scene> CreateInitScene(Group group)
        {
            var wakeupInitScene = new Scene
            {
                Name = Constants.Scenes.WakeupInit,
                Lights = group.Lights,
                Recycle = true
            };

            var wakeupInitSceneId = await _hueClient.CreateSceneAsync(wakeupInitScene);

            wakeupInitScene = await _hueClient.GetSceneAsync(wakeupInitSceneId);

            wakeupInitScene.Type = SceneType.GroupScene;
            wakeupInitScene.Group = group.Id;

            await _hueClient.UpdateSceneAsync(wakeupInitSceneId, wakeupInitScene);

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

        private async Task<Scene> CreateTransitionUpScene(Group group)
        {
            var wakeupTransitionUpScene = new Scene
            {
                Name = Constants.Scenes.WakeupTransitionUp,
                Lights = group.Lights,
                Recycle = true
            };

            var wakeupTransitionUpSceneId = await _hueClient.CreateSceneAsync(wakeupTransitionUpScene);

            wakeupTransitionUpScene = await _hueClient.GetSceneAsync(wakeupTransitionUpSceneId);

            wakeupTransitionUpScene.Type = SceneType.GroupScene;
            wakeupTransitionUpScene.Group = group.Id;

            await _hueClient.UpdateSceneAsync(wakeupTransitionUpSceneId, wakeupTransitionUpScene);

            foreach (var lightId in group.Lights)
            {
                await _hueClient.ModifySceneAsync(
                    wakeupTransitionUpSceneId,
                    lightId,
                    new LightCommand
                    {
                        On = true,
                        Brightness = 255,
                        ColorTemperature = 447,
                        TransitionTime = TimeSpan.FromMinutes(_settingsProvider.WakeupTransitionUpInMinutes)
                                                 .Subtract(TimeSpan.FromSeconds(Constants.ScheduleDeactivateDelayInSeconds))
                    });
            }

            Console.WriteLine($"Scene ({wakeupTransitionUpScene.Name}) with id {wakeupTransitionUpSceneId} created");

            return await _hueClient.GetSceneAsync(wakeupTransitionUpSceneId);
        }

        private async Task<Scene> CreateTransitionDownScene(Group group)
        {
            var wakeupTransitionDownScene = new Scene
            {
                Name = Constants.Scenes.WakeupTransitionDown,
                Lights = group.Lights,
                Recycle = true
            };

            var wakeupTransitionDownSceneId = await _hueClient.CreateSceneAsync(wakeupTransitionDownScene);

            wakeupTransitionDownScene = await _hueClient.GetSceneAsync(wakeupTransitionDownSceneId);

            wakeupTransitionDownScene.Type = SceneType.GroupScene;
            wakeupTransitionDownScene.Group = group.Id;

            await _hueClient.UpdateSceneAsync(wakeupTransitionDownSceneId, wakeupTransitionDownScene);

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
                                                 .Subtract(TimeSpan.FromSeconds(Constants.ScheduleDeactivateDelayInSeconds)),
                    });
            }

            Console.WriteLine($"Scene ({wakeupTransitionDownScene.Name}) with id {wakeupTransitionDownSceneId} created");

            return await _hueClient.GetSceneAsync(wakeupTransitionDownSceneId);
        }

        private async Task<Scene> CreateTurnOffScene(Group group)
        {
            var wakeupTurnOffScene = new Scene
            {
                Name = Constants.Scenes.WakeupTurnOff,
                Lights = group.Lights,
                Recycle = true
            };

            var wakeupTurnOffSceneId = await _hueClient.CreateSceneAsync(wakeupTurnOffScene);

            wakeupTurnOffScene = await _hueClient.GetSceneAsync(wakeupTurnOffSceneId);

            wakeupTurnOffScene.Type = SceneType.GroupScene;
            wakeupTurnOffScene.Group = group.Id;

            await _hueClient.UpdateSceneAsync(wakeupTurnOffSceneId, wakeupTurnOffScene);

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
}
