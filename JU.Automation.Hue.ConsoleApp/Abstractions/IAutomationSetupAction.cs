using Q42.HueApi;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JU.Automation.Hue.ConsoleApp.Abstractions
{
    public interface IAutomationSetupAction<TModel> where TModel : BaseModel
    {
        Task<TModel> Execute(TModel model);
    }

    public interface IWakeupAutomationSetupAction<TModel> : IAutomationSetupAction<TModel> where TModel : BaseModel { }

    public interface ISunriseAutomationSetupAction<TModel> : IAutomationSetupAction<TModel> where TModel : BaseModel { }

    public interface IBedtimeAutomationSetupAction<TModel> : IAutomationSetupAction<TModel> where TModel : BaseModel { }

    public interface IAllOffAutomationSetupAction<TModel> : IAutomationSetupAction<TModel> where TModel : BaseModel { }

    public abstract class BaseModel
    {
        public RecurringDay RecurringDay { get; set; }
        public Group Group { get; set; }
        public IList<Light> Lights { get; set; }
        public Sensor TriggerSensor { get; set; }
    }
}
