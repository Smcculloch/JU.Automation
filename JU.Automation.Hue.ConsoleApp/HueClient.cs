using System;
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
                Initialize(settingsProvider.AppKey);
        }

        public async Task<string> NewDeveloper(string appName, string deviceName)
        {
            string appKey = null;

            while (string.IsNullOrEmpty(appKey))
            {
                try
                {
                    appKey = await RegisterAsync(appName, deviceName);
                }
                catch (LinkButtonNotPressedException)
                {
                    Console.WriteLine("Press link button to generate app key! Press any key to continue ...");
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error {e.Message}");
                    throw;
                }
            }

            Initialize(appKey);

            return appKey;
        }
    }
}
