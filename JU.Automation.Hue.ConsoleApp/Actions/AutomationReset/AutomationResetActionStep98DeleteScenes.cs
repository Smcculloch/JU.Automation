using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Actions.AutomationReset
{
    public class AutomationResetActionStep98DeleteScenes : AutomationResetActionStepBase<AutomationResetActionStep98DeleteScenes>
    {
        private readonly IHueClient _hueClient;

        public AutomationResetActionStep98DeleteScenes(
            IHueClient hueClient,
            ILogger<AutomationResetActionStep98DeleteScenes> logger) : base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 98;

        public override async Task ExecuteStep()
        {
            var scenes = await _hueClient.GetScenesAsync();

            foreach (var scene in scenes)
            {
                await _hueClient.DeleteSceneAsync(scene.Id);
            }

            Console.WriteLine($"Deleted {scenes.Count} scenes");
        }
    }
}
