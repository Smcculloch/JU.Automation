using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Microsoft.Extensions.Logging;

namespace JU.Automation.Hue.ConsoleApp.Actions.Setup
{
    public abstract class SetupActionStepBase<T>: ActionStepBase, ISetupAction, IStep
    {
        private readonly ILogger<T> _logger;

        protected SetupActionStepBase(ILogger<T> logger)
        {
            _logger = logger;
        }

        public abstract int Step { get; }

        public abstract Task ExecuteStep();

        public async Task Execute()
        {
            await ExecuteStep();

            _logger.LogInformation($"Setup {GetType().Name} (step {Step}) completed");
        }
    }
}
