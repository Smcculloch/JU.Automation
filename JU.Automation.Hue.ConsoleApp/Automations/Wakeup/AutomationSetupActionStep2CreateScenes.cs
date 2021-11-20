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
    public class AutomationSetupActionStep2CreateScenes : AutomationSetupActionStepBase<AutomationSetupActionStep2CreateScenes, WakeupModel>
    {
        private readonly IHueClient _hueClient;
        private readonly ISettingsProvider _settingsProvider;

        public AutomationSetupActionStep2CreateScenes(
            IHueClient hueClient,
            ILogger<AutomationSetupActionStep2CreateScenes> logger,
            ISettingsProvider settingsProvider) : base(logger)
        {
            _hueClient = hueClient;
            _settingsProvider = settingsProvider;
        }

        public override int Step => 2;

        public override async Task<WakeupModel> ExecuteStep(WakeupModel model)
        {
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
            var wakeup1InitScene = new Scene
            {
                Name = Constants.Scenes.Wakeup1Init,
                Lights = group.Lights,
                Recycle = true
            };

            var wakeup1InitSceneId = await _hueClient.CreateSceneAsync(wakeup1InitScene);

            wakeup1InitScene = await _hueClient.GetSceneAsync(wakeup1InitSceneId);

            wakeup1InitScene.Type = SceneType.GroupScene;
            wakeup1InitScene.Group = group.Id;

            await _hueClient.UpdateSceneAsync(wakeup1InitSceneId, wakeup1InitScene);

            foreach (var lightId in group.Lights)
            {
                await _hueClient.ModifySceneAsync(
                    wakeup1InitSceneId,
                    lightId,
                    new LightCommand
                    {
                        On = true,
                        Brightness = 1,
                        ColorTemperature = 447,
                    });
            }

            Console.WriteLine($"Scene ({wakeup1InitScene.Name}) with id {wakeup1InitSceneId} created");

            return await _hueClient.GetSceneAsync(wakeup1InitSceneId);
        }

        private async Task<Scene> CreateTransitionUpScene(Group group)
        {
            var wakeup1TransitionUpScene = new Scene
            {
                Name = Constants.Scenes.Wakeup1TransitionUp,
                Lights = group.Lights,
                Recycle = true
            };

            var wakeup1TransitionUpSceneId = await _hueClient.CreateSceneAsync(wakeup1TransitionUpScene);

            wakeup1TransitionUpScene = await _hueClient.GetSceneAsync(wakeup1TransitionUpSceneId);

            wakeup1TransitionUpScene.Type = SceneType.GroupScene;
            wakeup1TransitionUpScene.Group = group.Id;

            await _hueClient.UpdateSceneAsync(wakeup1TransitionUpSceneId, wakeup1TransitionUpScene);

            foreach (var lightId in group.Lights)
            {
                await _hueClient.ModifySceneAsync(
                    wakeup1TransitionUpSceneId,
                    lightId,
                    new LightCommand
                    {
                        On = true,
                        Brightness = 255,
                        ColorTemperature = 447,
                        TransitionTime = TimeSpan.FromMinutes(_settingsProvider.WakeupTransitionUpInMinutes)
                    });
            }

            Console.WriteLine($"Scene ({wakeup1TransitionUpScene.Name}) with id {wakeup1TransitionUpSceneId} created");

            return await _hueClient.GetSceneAsync(wakeup1TransitionUpSceneId);
        }

        private async Task<Scene> CreateTransitionDownScene(Group group)
        {
            var wakeup1TransitionDownScene = new Scene
            {
                Name = Constants.Scenes.Wakeup1TurnOff,
                Lights = group.Lights,
                Recycle = true
            };

            var wakeup1TransitionDownSceneId = await _hueClient.CreateSceneAsync(wakeup1TransitionDownScene);

            wakeup1TransitionDownScene = await _hueClient.GetSceneAsync(wakeup1TransitionDownSceneId);

            wakeup1TransitionDownScene.Type = SceneType.GroupScene;
            wakeup1TransitionDownScene.Group = group.Id;

            await _hueClient.UpdateSceneAsync(wakeup1TransitionDownSceneId, wakeup1TransitionDownScene);

            foreach (var lightId in group.Lights)
            {
                await _hueClient.ModifySceneAsync(
                    wakeup1TransitionDownSceneId,
                    lightId,
                    new LightCommand
                    {
                        On = true,
                        Brightness = 1,
                        ColorTemperature = 447,
                        TransitionTime = TimeSpan.FromMinutes(_settingsProvider.WakeupTransitionDownInMinutes)
                    });
            }

            Console.WriteLine($"Scene ({wakeup1TransitionDownScene.Name}) with id {wakeup1TransitionDownSceneId} created");

            return await _hueClient.GetSceneAsync(wakeup1TransitionDownSceneId);
        }

        private async Task<Scene> CreateTurnOffScene(Group group)
        {
            var wakeup1TurnOffScene = new Scene
            {
                Name = Constants.Scenes.Wakeup1TurnOff,
                Lights = group.Lights,
                Recycle = true
            };

            var wakeup1TurnOffSceneId = await _hueClient.CreateSceneAsync(wakeup1TurnOffScene);

            wakeup1TurnOffScene = await _hueClient.GetSceneAsync(wakeup1TurnOffSceneId);

            wakeup1TurnOffScene.Type = SceneType.GroupScene;
            wakeup1TurnOffScene.Group = group.Id;

            await _hueClient.UpdateSceneAsync(wakeup1TurnOffSceneId, wakeup1TurnOffScene);

            foreach (var lightId in group.Lights)
            {
                await _hueClient.ModifySceneAsync(
                    wakeup1TurnOffSceneId,
                    lightId,
                    new LightCommand
                    {
                        On = false,
                        Brightness = 0
                    });
            }

            Console.WriteLine($"Scene ({wakeup1TurnOffScene.Name}) with id {wakeup1TurnOffSceneId} created");

            return await _hueClient.GetSceneAsync(wakeup1TurnOffSceneId);
        }
    }
}
