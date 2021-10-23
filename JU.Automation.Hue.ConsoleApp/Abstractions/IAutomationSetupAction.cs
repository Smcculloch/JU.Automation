using System.Threading.Tasks;

namespace JU.Automation.Hue.ConsoleApp.Abstractions
{
    public interface IAutomationSetupAction
    {
        Task<bool> Execute();
    }

    public interface IWakeupAutomationSetupAction : IAutomationSetupAction  { }
}
