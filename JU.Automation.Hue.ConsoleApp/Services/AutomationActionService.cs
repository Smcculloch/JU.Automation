using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;

namespace JU.Automation.Hue.ConsoleApp.Services
{
    public interface IAutomationActionService
    {
        Task<bool> CreateWakeupAutomation();
    }

    public class AutomationActionService: IAutomationActionService
    {
        private readonly IEnumerable<IAutomationSetupAction> _automationSetupActions;

        public AutomationActionService(IEnumerable<IWakeupAutomationSetupAction> wakeupAutomationSetupActions)
        {
            _automationSetupActions = wakeupAutomationSetupActions.Cast<IStep>()
                                                                  .OrderBy(actionStep => actionStep.Step)
                                                                  .Cast<IAutomationSetupAction>();
        }

        public async Task<bool> CreateWakeupAutomation()
        {
            var success = true;

            foreach (var automationSetupAction in _automationSetupActions)
            {
                success = await automationSetupAction.Execute();

                if (!success)
                    break;
            }

            return success;
        }
    }
}
