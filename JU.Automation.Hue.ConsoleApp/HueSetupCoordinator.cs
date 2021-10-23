using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Services;

namespace JU.Automation.Hue.ConsoleApp
{
    public interface IHueSetupCoordinator
    {
        Task GetConfig();
        Task CreateNewDeveloper();
        Task IdentifyLightsAsync();
        Task FullSetupAsync();
        Task FullSetupAdvancedAsync();
        Task CreateAutomationsAsync();
        Task FullResetAsync();
        Task ResetAutomationsAsync();
    }

    public class HueSetupCoordinator : IHueSetupCoordinator
    {
        private readonly IAutomationActionService _automationActionService;
        private readonly IGenericActionService _genericActionService;
        private readonly IResetActionService _resetActionService;
        private readonly ISetupActionService _setupActionService;

        public HueSetupCoordinator(
            IAutomationActionService automationActionService,
            IGenericActionService genericActionService,
            IResetActionService resetActionService,
            ISetupActionService setupActionService)
        {
            _automationActionService = automationActionService;
            _genericActionService = genericActionService;
            _resetActionService = resetActionService;
            _setupActionService = setupActionService;
        }

        public async Task GetConfig() => await _genericActionService.ShowConfig();

        public async Task CreateNewDeveloper() => await _setupActionService.NewDeveloper();

        public async Task IdentifyLightsAsync() => await _genericActionService.IdentifyLights();

        public async Task FullSetupAsync()
        {
            await _setupActionService.GetNewLights();

            var success = await _setupActionService.RunInitialSetup();

            if (success)
                await CreateAutomationsAsync();
        }

        public async Task FullSetupAdvancedAsync()
        {
            await _setupActionService.SearchNewLights();

            var success = await _setupActionService.RunInitialSetup();

            if (success)
                await CreateAutomationsAsync();
        }

        public async Task CreateAutomationsAsync() => await _automationActionService.CreateWakeupAutomation();

        public async Task FullResetAsync() => await _resetActionService.FullReset();

        public async Task ResetAutomationsAsync() => await _resetActionService.ResetAutomations();
    }
}
