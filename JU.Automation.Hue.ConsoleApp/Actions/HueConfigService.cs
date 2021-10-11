using System.Collections.Generic;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;

namespace JU.Automation.Hue.ConsoleApp.Actions
{
    public interface IHueConfigService
    {
        Task FullSetupAsync();
        Task CreateAutomationsAsync();
        Task FullResetAsync();
        Task ResetAutomationsAsync();
    }

    public class HueConfigService : IHueConfigService
    {
        private readonly IEnumerable<ISetupAction> _setupActions;
        private readonly IEnumerable<IAutomationSetupAction> _automationSetupActions;
        private readonly IEnumerable<IResetAction> _resetActions;
        private readonly IEnumerable<IAutomationResetAction> _automationResetActions;

        public HueConfigService(
            IEnumerable<ISetupAction> setupActions,
            IEnumerable<IAutomationSetupAction> automationSetupActions,
            IEnumerable<IResetAction> resetActions,
            IEnumerable<IAutomationResetAction> automationResetActions)
        {
            _setupActions = setupActions;
            _automationSetupActions = automationSetupActions;
            _resetActions = resetActions;
            _automationResetActions = automationResetActions;
        }

        public async Task FullSetupAsync()
        {
            foreach (var setupAction in _setupActions)
            {
                await setupAction.Execute();
            }

            await CreateAutomationsAsync();
        }

        public async Task CreateAutomationsAsync()
        {
            foreach (var automationSetupAction in _automationSetupActions)
            {
                await automationSetupAction.Execute();
            }
        }

        public async Task FullResetAsync()
        {
            await ResetAutomationsAsync();

            foreach (var resetAction in _resetActions)
            {
                await resetAction.Execute();
            }
        }

        public async Task ResetAutomationsAsync()
        {
            foreach (var automationResetAction in _automationResetActions)
            {
                await automationResetAction.Execute();
            }
        }
    }
}
