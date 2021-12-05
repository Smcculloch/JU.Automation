using System;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Automations.Bedtime
{
    public class ActionStep2CreateScenes : ActionStepBase<ActionStep2CreateScenes, BedtimeModel>
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

        public override async Task<BedtimeModel> ExecuteStep(BedtimeModel model)
        {
            if (model.RecurringDay == default)
                throw new ArgumentException($"{nameof(model.RecurringDay)} is invalid");

            if (model.BedtimeTime == TimeSpan.Zero)
                throw new ArgumentException($"{nameof(model.BedtimeTime)} is invalid");

            if (model.Group == null)
                throw new ArgumentNullException($"{model.Group} cannot be null");

            if (model.Lights == null)
                throw new ArgumentNullException($"{model.Lights} cannot be null");

            if (model.TriggerSensor == null)
                throw new ArgumentNullException($"{model.TriggerSensor} cannot be null");

            model.Scenes.Init = await CreateInitScene(model.Group);
            model.Scenes.TransitionUp = await CreateTransitionUpScene(model.Group);
            model.Scenes.TransitionDown1 = await CreateTransitionDown1Scene(model.Group);
            model.Scenes.TransitionDown2 = await CreateTransitionDown2Scene(model.Group);
            model.Scenes.TurnOff = await CreateTurnOffScene(model.Group);

            return model;
        }

        private async Task<Scene> CreateInitScene(Group group)
        {
            var bedtimeInitScene = new Scene
            {
                Name = Constants.Scenes.BedtimeInit,
                Type = SceneType.GroupScene,
                Group = group.Id
            };

            var bedtimeInitSceneId = await _hueClient.CreateSceneAsync(bedtimeInitScene);

            foreach (var lightId in group.Lights)
            {
                await _hueClient.ModifySceneAsync(
                    bedtimeInitSceneId,
                    lightId,
                    new LightCommand
                    {
                        On = true,
                        Brightness = 1,
                        ColorTemperature = 447
                    });
            }

            Console.WriteLine($"Scene ({bedtimeInitScene.Name}) with id {bedtimeInitSceneId} created");

            return await _hueClient.GetSceneAsync(bedtimeInitSceneId);
        }

        private async Task<Scene> CreateTransitionUpScene(Group group)
        {
            var bedtimeTransitionUpScene = new Scene
            {
                Name = Constants.Scenes.BedtimeTransitionUp,
                Type = SceneType.GroupScene,
                Group = group.Id
            };

            var bedtimeTransitionUpSceneId = await _hueClient.CreateSceneAsync(bedtimeTransitionUpScene);

            foreach (var lightId in group.Lights)
            {
                await _hueClient.ModifySceneAsync(
                    bedtimeTransitionUpSceneId,
                    lightId,
                    new LightCommand
                    {
                        On = true,
                        Brightness = 255,
                        TransitionTime = TimeSpan.FromMinutes(_settingsProvider.EveningLightsOnTransitionUpInMinutes)
                                                 .Subtract(TimeSpan.FromSeconds(Constants.ScheduleDeactivateDelayInSeconds))
                    });
            }

            Console.WriteLine($"Scene ({bedtimeTransitionUpScene.Name}) with id {bedtimeTransitionUpSceneId} created");

            return await _hueClient.GetSceneAsync(bedtimeTransitionUpSceneId);
        }

        private async Task<Scene> CreateTransitionDown1Scene(Group group)
        {
            var bedtimeTransitionDown1Scene = new Scene
            {
                Name = Constants.Scenes.BedtimeTransitionDown1,
                Type = SceneType.GroupScene,
                Group = group.Id
            };

            var bedtimeTransitionDown1SceneId = await _hueClient.CreateSceneAsync(bedtimeTransitionDown1Scene);

            foreach (var lightId in group.Lights)
            {
                await _hueClient.ModifySceneAsync(
                    bedtimeTransitionDown1SceneId,
                    lightId,
                    new LightCommand
                    {
                        On = true,
                        Brightness = 85,
                        TransitionTime = TimeSpan.FromMinutes(_settingsProvider.BedtimeTransitionDown1InMinutes)
                                                 .Subtract(TimeSpan.FromSeconds(Constants.ScheduleDeactivateDelayInSeconds)),
                    });
            }

            Console.WriteLine($"Scene ({bedtimeTransitionDown1Scene.Name}) with id {bedtimeTransitionDown1SceneId} created");

            return await _hueClient.GetSceneAsync(bedtimeTransitionDown1SceneId);
        }

        private async Task<Scene> CreateTransitionDown2Scene(Group group)
        {
            var bedtimeTransitionDown2Scene = new Scene
            {
                Name = Constants.Scenes.BedtimeTransitionDown2,
                Type = SceneType.GroupScene,
                Group = group.Id
            };

            var bedtimeTransitionDown2SceneId = await _hueClient.CreateSceneAsync(bedtimeTransitionDown2Scene);

            foreach (var lightId in group.Lights)
            {
                await _hueClient.ModifySceneAsync(
                    bedtimeTransitionDown2SceneId,
                    lightId,
                    new LightCommand
                    {
                        On = true,
                        Brightness = 1,
                        TransitionTime = TimeSpan.FromMinutes(_settingsProvider.BedtimeTransitionDown2InMinutes)
                                                 .Subtract(TimeSpan.FromSeconds(Constants.ScheduleDeactivateDelayInSeconds)),
                    });
            }

            Console.WriteLine($"Scene ({bedtimeTransitionDown2Scene.Name}) with id {bedtimeTransitionDown2SceneId} created");

            return await _hueClient.GetSceneAsync(bedtimeTransitionDown2SceneId);
        }

        private async Task<Scene> CreateTurnOffScene(Group group)
        {
            var bedtimeTurnOffScene = new Scene
            {
                Name = Constants.Scenes.BedtimeTurnOff,
                Type = SceneType.GroupScene,
                Group = group.Id
            };

            var bedtimeTurnOffSceneId = await _hueClient.CreateSceneAsync(bedtimeTurnOffScene);

            foreach (var lightId in group.Lights)
            {
                await _hueClient.ModifySceneAsync(
                    bedtimeTurnOffSceneId,
                    lightId,
                    new LightCommand
                    {
                        On = false,
                        Brightness = 0
                    });
            }

            Console.WriteLine($"Scene ({bedtimeTurnOffScene.Name}) with id {bedtimeTurnOffSceneId} created");

            return await _hueClient.GetSceneAsync(bedtimeTurnOffSceneId);
        }
    }
}
