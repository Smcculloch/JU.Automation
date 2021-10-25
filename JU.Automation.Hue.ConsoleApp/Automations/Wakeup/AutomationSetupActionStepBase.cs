using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Microsoft.Extensions.Logging;

namespace JU.Automation.Hue.ConsoleApp.Automations.Wakeup
{
    public abstract class AutomationSetupActionStepBase<T, TModel>: IWakeupAutomationSetupAction<TModel>, IStep where TModel : class
    {
        private readonly ILogger<T> _logger;

        protected AutomationSetupActionStepBase(ILogger<T> logger)
        {
            _logger = logger;
        }

        public abstract int Step { get; }

        public abstract Task<TModel> ExecuteStep(TModel model);

        public async Task<TModel> Execute(TModel model)
        {
            var result = await ExecuteStep(model);

            _logger.LogInformation($"Automation Setup {GetType().Name} (step {Step}) completed");

            return result;
        }
    }
}
