using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Actions.AutomationSetup
{
    public class AutomationSetupActionStep2CreateScenes : AutomationSetupActionStepBase<AutomationSetupActionStep2CreateScenes>
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

        public override async Task ExecuteStep()
        {
            var lights = await GetGroupLights(Constants.Groups.Bedroom);

            var wakeup1InitScene = new Scene
            {
                Name = Constants.Scenes.Wakeup1Init,
                Lights = lights
            };

            var wakeup1InitSceneId = await _hueClient.CreateSceneAsync(wakeup1InitScene);

            Console.WriteLine($"Scene ({wakeup1InitScene.Name}) with id {wakeup1InitSceneId} created");

            var wakeup1EndScene = new Scene
            {
                Name = Constants.Scenes.Wakeup1End,
                Lights = lights
            };

            var wakeup1EndSceneId = await _hueClient.CreateSceneAsync(wakeup1EndScene);

            Console.WriteLine($"Scene ({wakeup1EndScene.Name}) with id {wakeup1EndSceneId} created");
        }

        private async Task<IEnumerable<string>> GetGroupLights(string groupName)
        {
            var groups = await _hueClient.GetGroupsAsync();

            return groups.SingleOrDefault(g => g.Name == groupName)?.Lights ?? Enumerable.Empty<string>();
        }
    }
}
