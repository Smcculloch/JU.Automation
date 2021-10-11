using System.Net.Http;
using JU.Automation.Hue.ConsoleApp.Providers;
using Q42.HueApi;

namespace JU.Automation.Hue.ConsoleApp
{
    public class HueClient : LocalHueClient
    {
        public HueClient(
            ISettingsProvider settingsProvider,
            HttpClient client) : base(settingsProvider.LocalHueClientIp, client)
        {
            this.Initialize(settingsProvider.AppKey);
        }
    }
}
