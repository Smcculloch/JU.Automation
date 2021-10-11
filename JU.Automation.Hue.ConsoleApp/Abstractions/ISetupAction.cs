using System.Threading.Tasks;

namespace JU.Automation.Hue.ConsoleApp.Abstractions
{
    public interface ISetupAction
    {
        Task Execute();
    }
}
