using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Microsoft.Extensions.Logging;

namespace JU.Automation.Hue.ConsoleApp.Actions.Initial
{
    public abstract class SetupActionStepBase<T> : IInitialSetupAction, IStep
    {
        private readonly ILogger<T> _logger;

        protected SetupActionStepBase(ILogger<T> logger)
        {
            _logger = logger;
        }

        public abstract int Step { get; }

        public abstract Task<bool> ExecuteStep();

        public async Task<bool> Execute()
        {
            var success = await ExecuteStep();

            _logger.LogInformation($"Setup {GetType().Name} (step {Step}) completed");

            return success;
        }
    }
}
