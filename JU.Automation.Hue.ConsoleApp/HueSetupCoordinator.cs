using System;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
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
        private readonly IUserInputService _userInputService;

        public HueSetupCoordinator(
            IAutomationActionService automationActionService,
            IGenericActionService genericActionService,
            IResetActionService resetActionService,
            ISetupActionService setupActionService,
            IUserInputService userInputService)
        {
            _automationActionService = automationActionService;
            _genericActionService = genericActionService;
            _resetActionService = resetActionService;
            _setupActionService = setupActionService;
            _userInputService = userInputService;
        }

        public async Task GetConfig() => await _genericActionService.ShowConfig();

        public async Task CreateNewDeveloper() => await _setupActionService.NewDeveloper();

        public async Task IdentifyLightsAsync() => await _genericActionService.IdentifyLights();

        public async Task FullSetupAsync()
        {
            var result = await _setupActionService.GetNewLights();

            if (!result.Success)
            {
                Console.WriteLine($"Error(s) when searching for new lights:");
                foreach (var error in result.Errors)
                    Console.WriteLine(error);
                Console.WriteLine($"Resolve errors and re-run setup to continue");
                return;
            }

            var success = await _setupActionService.RunInitialSetup();

            if (success)
                await CreateAutomationsAsync();
        }

        public async Task FullSetupAdvancedAsync()
        {
            var result = await _setupActionService.SearchNewLights();

            if (!result.Success)
            {
                Console.WriteLine($"Error(s) when searching for new lights:");
                foreach (var error in result.Errors)
                    Console.WriteLine(error);
                Console.WriteLine($"Resolve errors and re-run setup to continue");

                return;
            }

            var success = await _setupActionService.RunInitialSetup();

            if (success)
                await CreateAutomationsAsync();
        }

        public async Task CreateAutomationsAsync()
        {
            var wakeupTime = _userInputService.PromptWakeupTime();
            var departureTime = _userInputService.PromptDepartureTime();
            var bedtime = _userInputService.PromptBedtime();

            await _automationActionService.Wakeup(Constants.Groups.Bedroom, wakeupTime);
            await _automationActionService.Sunrise(Constants.Groups.Kitchen, wakeupTime, departureTime);
            await _automationActionService.Bedtime(Constants.Groups.LivingRoom, bedtime);
        }

        public async Task FullResetAsync() => await _resetActionService.FullReset();

        public async Task ResetAutomationsAsync() => await _resetActionService.ResetAutomations();
    }
}
