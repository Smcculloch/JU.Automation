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
            ISettingsProvider settingsProvider): base(logger)
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
            model.Scenes.Wakeup = await CreateWakeupScene(model.Group);
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

        private async Task<Scene> CreateWakeupScene(Group group)
        {
            var wakeup1WakeupScene = new Scene
            {
                Name = Constants.Scenes.Wakeup1Wakeup,
                Lights = group.Lights,
                Recycle = true
            };

            var wakeup1WakeupSceneId = await _hueClient.CreateSceneAsync(wakeup1WakeupScene);

            wakeup1WakeupScene = await _hueClient.GetSceneAsync(wakeup1WakeupSceneId);

            wakeup1WakeupScene.Type = SceneType.GroupScene;
            wakeup1WakeupScene.Group = group.Id;

            await _hueClient.UpdateSceneAsync(wakeup1WakeupSceneId, wakeup1WakeupScene);

            foreach (var lightId in group.Lights)
            {
                await _hueClient.ModifySceneAsync(
                    wakeup1WakeupSceneId,
                    lightId,
                    new LightCommand
                    {
                        On = true,
                        Brightness = 255,
                        ColorTemperature = 447,
                        TransitionTime = TimeSpan.FromMinutes(_settingsProvider.WakeupTransitionInMinutes)
                    });
            }

            Console.WriteLine($"Scene ({wakeup1WakeupScene.Name}) with id {wakeup1WakeupSceneId} created");

            return await _hueClient.GetSceneAsync(wakeup1WakeupSceneId);
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
                        On = true,
                        Brightness = 1,
                        ColorTemperature = 447,
                        TransitionTime = TimeSpan.FromMinutes(_settingsProvider.TurnOffTransitionInMinutes)
                    });
            }

            Console.WriteLine($"Scene ({wakeup1TurnOffScene.Name}) with id {wakeup1TurnOffSceneId} created");

            return await _hueClient.GetSceneAsync(wakeup1TurnOffSceneId);
        }
    }
}
