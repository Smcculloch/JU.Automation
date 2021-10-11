using System.Threading.Tasks;

namespace JU.Automation.Hue.ConsoleApp.Abstractions
{
    public interface IResetAction
    {
        Task Execute();
    }
}
