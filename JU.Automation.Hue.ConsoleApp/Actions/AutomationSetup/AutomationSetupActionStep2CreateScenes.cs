using System;
using System.Linq;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Actions.AutomationSetup
{
    public class AutomationSetupActionStep2CreateScenes : AutomationSetupActionStepBase<AutomationSetupActionStep2CreateScenes>
    {
        private readonly IHueClient _hueClient;

        public AutomationSetupActionStep2CreateScenes(
            IHueClient hueClient,
            ILogger<AutomationSetupActionStep2CreateScenes> logger,
            ISettingsProvider settingsProvider): base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 2;

        public override async Task ExecuteStep()
        {
            var groupBedroom = await GetGroup(Constants.Groups.Bedroom);

            if (groupBedroom?.Lights == null)
                throw new ArgumentNullException($"{Constants.Groups.Bedroom} ({nameof(Group.Lights)}) cannot be null");

            var wakeup1InitScene = new Scene
            {
                Name = Constants.Scenes.Wakeup1Init,
                Lights = groupBedroom.Lights,
                Recycle = true
            };

            var wakeup1InitSceneId = await _hueClient.CreateSceneAsync(wakeup1InitScene);

            wakeup1InitScene = await _hueClient.GetSceneAsync(wakeup1InitSceneId);

            wakeup1InitScene.Type = SceneType.GroupScene;
            wakeup1InitScene.Group = groupBedroom.Id;

            await _hueClient.UpdateSceneAsync(wakeup1InitSceneId, wakeup1InitScene);

            foreach (var lightId in groupBedroom.Lights)
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

            var wakeup1EndScene = new Scene
            {
                Name = Constants.Scenes.Wakeup1End,
                Lights = groupBedroom.Lights,
                Recycle = true
            };

            var wakeup1EndSceneId = await _hueClient.CreateSceneAsync(wakeup1EndScene);

            wakeup1EndScene = await _hueClient.GetSceneAsync(wakeup1EndSceneId);

            wakeup1EndScene.Type = SceneType.GroupScene;
            wakeup1EndScene.Group = groupBedroom.Id;

            await _hueClient.UpdateSceneAsync(wakeup1EndSceneId, wakeup1EndScene);

            foreach (var lightId in groupBedroom.Lights)
            {
                await _hueClient.ModifySceneAsync(
                    wakeup1EndSceneId,
                    lightId,
                    new LightCommand
                    {
                        On = true,
                        Brightness = 255,
                        ColorTemperature = 447,
                        TransitionTime = TimeSpan.FromMinutes(15)
                    });
            }

            Console.WriteLine($"Scene ({wakeup1EndScene.Name}) with id {wakeup1EndSceneId} created");
        }

        private async Task<Group> GetGroup(string groupName)
        {
            var groups = await _hueClient.GetGroupsAsync();

            return groups.SingleOrDefault(g => g.Name == groupName);
        }
    }
}
