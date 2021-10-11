using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Microsoft.Extensions.Logging;

namespace JU.Automation.Hue.ConsoleApp.Actions.Reset
{
    public abstract class ResetActionStepBase<T>: ActionStepBase, IResetAction, IStep
    {
        private readonly ILogger<T> _logger;

        protected ResetActionStepBase(ILogger<T> logger)
        {
            _logger = logger;
        }

        public abstract int Step { get; }

        public abstract Task ExecuteStep();

        public async Task Execute()
        {
            await ExecuteStep();

            _logger.LogInformation($"Reset {GetType().Name} (step {Step}) completed");
        }
    }
}
