using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Microsoft.Extensions.Logging;

namespace JU.Automation.Hue.ConsoleApp.Automations.Bedtime
{
    public abstract class ActionStepBase<T, TModel>: IBedtimeAutomationSetupAction<TModel>, IStep where TModel : BaseModel
    {
        private readonly ILogger<T> _logger;

        protected ActionStepBase(ILogger<T> logger)
        {
            _logger = logger;
        }

        public abstract int Step { get; }

        public abstract Task<TModel> ExecuteStep(TModel model);

        public async Task<TModel> Execute(TModel model)
        {
            var result = await ExecuteStep(model);

            _logger.LogInformation($"Bedtime automation setup {GetType().Name} (step {Step}) completed");

            return result;
        }
    }
}
