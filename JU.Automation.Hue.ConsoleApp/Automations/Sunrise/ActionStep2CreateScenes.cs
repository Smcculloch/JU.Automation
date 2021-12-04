using System;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Automations.Sunrise
{
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

            model.Scenes.Init = await CreateInitScene(model.Group);
            model.Scenes.TransitionUp = await CreateTransitionUpScene(model.Group);
            model.Scenes.TurnOff = await CreateTurnOffScene(model.Group);

            return model;
        }

        private async Task<Scene> CreateInitScene(Group group)
        {
            var sunriseInitScene = new Scene
            {
                Name = Constants.Scenes.SunriseInit,
                Lights = group.Lights,
                Recycle = true
            };

            var sunriseInitSceneId = await _hueClient.CreateSceneAsync(sunriseInitScene);

            sunriseInitScene = await _hueClient.GetSceneAsync(sunriseInitSceneId);

            sunriseInitScene.Type = SceneType.GroupScene;
            sunriseInitScene.Group = group.Id;

            await _hueClient.UpdateSceneAsync(sunriseInitSceneId, sunriseInitScene);

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

        private async Task<Scene> CreateTransitionUpScene(Group group)
        {
            var sunriseTransitionUpScene = new Scene
            {
                Name = Constants.Scenes.SunriseTransitionUp,
                Lights = group.Lights,
                Recycle = true
            };

            var sunriseTransitionUpSceneId = await _hueClient.CreateSceneAsync(sunriseTransitionUpScene);

            sunriseTransitionUpScene = await _hueClient.GetSceneAsync(sunriseTransitionUpSceneId);

            sunriseTransitionUpScene.Type = SceneType.GroupScene;
            sunriseTransitionUpScene.Group = group.Id;

            await _hueClient.UpdateSceneAsync(sunriseTransitionUpSceneId, sunriseTransitionUpScene);

            foreach (var lightId in group.Lights)
            {
                await _hueClient.ModifySceneAsync(
                    sunriseTransitionUpSceneId,
                    lightId,
                    new LightCommand
                    {
                        On = true,
                        Brightness = 255,
                        ColorTemperature = 447,
                        TransitionTime = TimeSpan.FromMinutes(_settingsProvider.SunriseTransitionUpInMinutes)
                                                 .Subtract(TimeSpan.FromSeconds(Constants.ScheduleDeactivateDelayInSeconds))
                    });
            }

            Console.WriteLine($"Scene ({sunriseTransitionUpScene.Name}) with id {sunriseTransitionUpSceneId} created");

            return await _hueClient.GetSceneAsync(sunriseTransitionUpSceneId);
        }

        private async Task<Scene> CreateTurnOffScene(Group group)
        {
            var sunriseTurnOffScene = new Scene
            {
                Name = Constants.Scenes.SunriseTurnOff,
                Lights = group.Lights,
                Recycle = true
            };

            var sunriseTurnOffSceneId = await _hueClient.CreateSceneAsync(sunriseTurnOffScene);

            sunriseTurnOffScene = await _hueClient.GetSceneAsync(sunriseTurnOffSceneId);

            sunriseTurnOffScene.Type = SceneType.GroupScene;
            sunriseTurnOffScene.Group = group.Id;

            await _hueClient.UpdateSceneAsync(sunriseTurnOffSceneId, sunriseTurnOffScene);

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
}
