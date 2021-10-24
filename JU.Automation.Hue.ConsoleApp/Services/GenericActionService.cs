using System;
using System.Linq;
using System.Threading.Tasks;
using Q42.HueApi;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Services
{
    public interface IGenericActionService
    {
        Task ShowConfig();
        Task IdentifyLights();
    }

    public class GenericActionService: IGenericActionService
    {
        private readonly IBridgeLocator _bridgeLocator;
        private readonly IHueClient _hueClient;

        public GenericActionService(
            IBridgeLocator bridgeLocator,
            IHueClient hueClient)
        {
            _bridgeLocator = bridgeLocator;
            _hueClient = hueClient;
        }

        public async Task ShowConfig()
        {
            var locatedBridges = await _bridgeLocator.LocateBridgesAsync(TimeSpan.FromSeconds(5));

            foreach (var locatedBridge in locatedBridges)
                Console.WriteLine($"Discovered {locatedBridge.BridgeId} ({locatedBridge.IpAddress})");
        }

        public async Task IdentifyLights()
        {
            var groups = (await _hueClient.GetGroupsAsync()).ToDictionary(group => group.Id);

            ConsoleKeyInfo continueIdentify;
            do
            {
                Console.WriteLine("Groups:");
                foreach (var @group in groups.Values)
                    Console.WriteLine($"({group.Id}) {group.Name}");
                Console.Write("Select group number (#): ");
                var groupId = Console.ReadLine();

                if (!groups.ContainsKey(groupId))
                    Console.WriteLine("Invalid input");
                else
                    await _hueClient.SendCommandAsync(new LightCommand { Alert = Alert.Multiple },
                        groups[groupId].Lights);

                Console.Write("Identify another group? (Y/N) ");
                continueIdentify = Console.ReadKey();
                Console.WriteLine();

                await _hueClient.SendCommandAsync(new LightCommand { Alert = Alert.None },
                    groups[groupId].Lights);

            } while (continueIdentify.Key == ConsoleKey.Y);
        }
    }
}
