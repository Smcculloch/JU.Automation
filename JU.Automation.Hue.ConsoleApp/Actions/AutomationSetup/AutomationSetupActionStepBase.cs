using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Microsoft.Extensions.Logging;

namespace JU.Automation.Hue.ConsoleApp.Actions.AutomationSetup
{
    public abstract class AutomationSetupActionStepBase<T>: ActionStepBase, IAutomationSetupAction, IStep
    {
        private readonly ILogger<T> _logger;

        protected AutomationSetupActionStepBase(ILogger<T> logger)
        {
            _logger = logger;
        }

        public abstract int Step { get; }

        public abstract Task ExecuteStep();

        public async Task Execute()
        {
            await ExecuteStep();

            _logger.LogInformation($"Automation Setup {GetType().Name} (step {Step}) completed");
        }
    }
}
