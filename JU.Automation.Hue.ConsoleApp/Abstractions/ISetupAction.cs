using System.Threading.Tasks;

namespace JU.Automation.Hue.ConsoleApp.Abstractions
{
    public interface ISetupAction
    {
        Task<bool> Execute();
    }

    public interface IInitialSetupAction : ISetupAction { }
}
