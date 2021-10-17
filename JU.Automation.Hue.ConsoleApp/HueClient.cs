using System.Net.Http;
using System.Threading.Tasks;
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
            if (!string.IsNullOrEmpty(settingsProvider.AppKey))
            {
                Initialize(settingsProvider.AppKey);
            }
        }

        public async Task<string> NewDeveloper(string appName, string deviceName)
        {
            var result = await RegisterAsync(appName, deviceName);

            if (!IsInitialized && !string.IsNullOrEmpty(result))
                Initialize(result);

            return result;
        }
    }
}
