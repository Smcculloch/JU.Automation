using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Automations.Wakeup;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Services
{
    public interface IAutomationActionService
    {
        Task<bool> CreateBedroomWakeupAutomation();
    }

    public class AutomationActionService: IAutomationActionService
    {
        private readonly IHueClient _hueClient;
        private readonly IEnumerable<IAutomationSetupAction<WakeupModel>> _automationSetupActions;

        public AutomationActionService(
            IHueClient hueClient,
            IEnumerable<IWakeupAutomationSetupAction<WakeupModel>> wakeupAutomationSetupActions)
        {
            _hueClient = hueClient;
            _automationSetupActions = wakeupAutomationSetupActions.Cast<IStep>()
                                                                  .OrderBy(actionStep => actionStep.Step)
                                                                  .Cast<IAutomationSetupAction<WakeupModel>>();
        }

        public async Task<bool> CreateBedroomWakeupAutomation()
        {
            var bedroomGroup = await GetGroup(Constants.Groups.Bedroom);
            var bedroomLights = await GetLights(bedroomGroup.Lights);

            var model = new WakeupModel
            {
                Group = bedroomGroup,
                Lights = bedroomLights
            };

            foreach (var automationSetupAction in _automationSetupActions)
            {
                model = await automationSetupAction.Execute(model);

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
