using System;
using System.Linq;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Services
{
    public interface IResetActionService
    {
        Task ResetSetup();
        Task ResetAutomations();
        Task FullReset();
        Task ResetSwitch();
    }

    public class ResetActionService: IResetActionService
    {
        private readonly IHueClient _hueClient;

        public ResetActionService(IHueClient hueClient)
        {
            _hueClient = hueClient;
        }

        public async Task ResetSetup()
        {
            var groups = await _hueClient.GetGroupsAsync();
            foreach (var group in groups)
                await _hueClient.DeleteGroupAsync(group.Id);
            Console.WriteLine($"Deleted {groups.Count} groups");

            var lights = await _hueClient.GetLightsAsync();
            foreach (var light in lights)
                await _hueClient.DeleteLightAsync(light.Id);
            Console.WriteLine($"Deleted {lights.Count()} lights");
        }

        public async Task ResetAutomations()
        {
            var resourceLinks = await _hueClient.GetResourceLinksAsync();
            foreach (var resourceLink in resourceLinks)
                await _hueClient.DeleteResourceLinkAsync(resourceLink.Id);
            Console.WriteLine($"Deleted {resourceLinks.Count} resourceLinks");

            var rules = await _hueClient.GetRulesAsync();
            foreach (var rule in rules)
                await _hueClient.DeleteRule(rule.Id);
            Console.WriteLine($"Deleted {rules.Count} rules");

            var schedules = await _hueClient.GetSchedulesAsync();
            foreach (var schedule in schedules)
                await _hueClient.DeleteScheduleAsync(schedule.Id);
            Console.WriteLine($"Deleted {schedules.Count} schedules");

            var scenes = await _hueClient.GetScenesAsync();
            foreach (var scene in scenes)
                await _hueClient.DeleteSceneAsync(scene.Id);
            Console.WriteLine($"Deleted {scenes.Count} scenes");

            var sensors = await _hueClient.GetSensorsAsync();
            foreach (var sensor in sensors)
                await _hueClient.DeleteSensorAsync(sensor.Id);
            Console.WriteLine($"Deleted {sensors.Count} sensors");
        }

        public async Task FullReset()
        {
            await ResetAutomations();
            await ResetSetup();
        }

        public async Task ResetSwitch()
        {
            var rules = await _hueClient.GetRulesAsync();
            var allOffRule = rules.SingleOrDefault(rule => rule.Name == Constants.Rules.AllOff);

            if (allOffRule != null)
            {
                await _hueClient.DeleteRule(allOffRule.Id);
                Console.WriteLine($"Deleted rule {allOffRule.Name}");
            }

            var scenes = await _hueClient.GetScenesAsync();
            var allOffScene = scenes.SingleOrDefault(scene => scene.Name == Constants.Scenes.AllOff);

            if (allOffScene != null)
            {
                await _hueClient.DeleteSceneAsync(allOffScene.Id);
                Console.WriteLine($"Deleted scene {allOffScene.Name}");
            }

            var sensors = await _hueClient.GetSensorsAsync();
            var allOffSensor = sensors.SingleOrDefault(sensor => sensor.Name == Constants.Switches.AllOff);

            if (allOffSensor != null)
            {
                await _hueClient.DeleteSensorAsync(allOffSensor.Id);
                Console.WriteLine($"Deleted sensor {allOffSensor.Name}");
            }
        }
    }
}
