using System.Threading.Tasks;

namespace JU.Automation.Hue.ConsoleApp.Abstractions
{
    public interface IAutomationSetupAction<TModel> where TModel : class
    {
        Task<TModel> Execute(TModel model);
    }

    public interface IWakeupAutomationSetupAction<TModel> : IAutomationSetupAction<TModel> where TModel : class { }
}
