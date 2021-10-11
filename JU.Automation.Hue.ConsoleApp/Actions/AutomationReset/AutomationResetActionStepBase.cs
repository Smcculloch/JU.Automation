using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Microsoft.Extensions.Logging;

namespace JU.Automation.Hue.ConsoleApp.Actions.AutomationReset
{
    public abstract class AutomationResetActionStepBase<T>: ActionStepBase, IAutomationResetAction, IStep
    {
        private readonly ILogger<T> _logger;

        protected AutomationResetActionStepBase(ILogger<T> logger)
        {
            _logger = logger;
        }

        public abstract int Step { get; }

        public abstract Task ExecuteStep();

        public async Task Execute()
        {
            await ExecuteStep();

            _logger.LogInformation($"Automation Reset {GetType().Name} (step {Step}) completed");
        }
    }
}
