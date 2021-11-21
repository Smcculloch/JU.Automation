using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Automations.Sunrise;
using JU.Automation.Hue.ConsoleApp.Automations.Wakeup;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Services
{
    public interface IAutomationActionService
    {
        Task<bool> Wakeup(string groupName, TimeSpan wakeupTime);
        Task<bool> Sunrise(string groupName, TimeSpan wakeupTime, TimeSpan departureTime);
    }

    public class AutomationActionService: IAutomationActionService
    {
        private readonly IHueClient _hueClient;
        private readonly IEnumerable<IAutomationSetupAction<WakeupModel>> _wakeupAutomationSetupActions;
        private readonly IEnumerable<IAutomationSetupAction<SunriseModel>> _sunriseAutomationSetupActions;

        public AutomationActionService(
            IHueClient hueClient,
            IEnumerable<IWakeupAutomationSetupAction<WakeupModel>> wakeupAutomationSetupActions,
            IEnumerable<ISunriseAutomationSetupAction<SunriseModel>> sunriseAutomationSetupActions)
        {
            _hueClient = hueClient;

            _wakeupAutomationSetupActions = wakeupAutomationSetupActions.Cast<IStep>()
                                                                        .OrderBy(actionStep => actionStep.Step)
                                                                        .Cast<IAutomationSetupAction<WakeupModel>>();

            _sunriseAutomationSetupActions = sunriseAutomationSetupActions.Cast<IStep>()
                                                                          .OrderBy(actionStep => actionStep.Step)
                                                                          .Cast<IAutomationSetupAction<SunriseModel>>();
        }

        public async Task<bool> Wakeup(string groupName, TimeSpan wakeupTime)
        {
            var group = await GetGroup(groupName);
            var lights = await GetLights(group.Lights);

            var model = new WakeupModel
            {
                WakeupTime = wakeupTime,
                Group = group,
                Lights = lights
            };

            foreach (var action in _wakeupAutomationSetupActions)
            {
                model = await action.Execute(model);

                if (model == null)
                    break;
            }

            return true;
        }

        public async Task<bool> Sunrise(string groupName, TimeSpan wakeupTime, TimeSpan departureTime)
        {
            var group = await GetGroup(groupName);
            var lights = await GetLights(group.Lights);

            var model = new SunriseModel
            {
                WakeupTime = wakeupTime,
                DepartureTime = departureTime,
                Group = group,
                Lights = lights
            };

            foreach (var action in _sunriseAutomationSetupActions)
            {
                model = await action.Execute(model);

                if (model == null)
                    break;
            }

            return true;
        }

        private async Task<Group> GetGroup(string groupName)
        {
            var groups = await _hueClient.GetGroupsAsync();

            return groups.SingleOrDefault(g => g.Name == groupName);
        }

        private async Task<IList<Light>> GetLights(IList<string> lightIds)
        {
            var lights = new List<Light>(lightIds.Count);

            foreach (var lightId in lightIds)
            {
                var light = await _hueClient.GetLightAsync(lightId);
                lights.Add(light);
            }

            return lights;
        }
    }
}
