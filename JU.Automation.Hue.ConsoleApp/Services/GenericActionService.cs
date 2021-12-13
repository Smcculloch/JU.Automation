using System;
using System.Linq;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Providers;
using Q42.HueApi;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Services;

public interface IGenericActionService
{
    Task ShowCapabilities();
    Task IdentifyLights();
}

public class GenericActionService : IGenericActionService
{
    private readonly IBridgeLocator _bridgeLocator;
    private readonly IHueClient _hueClient;
    private readonly ISettingsProvider _settingsProvider;

    public GenericActionService(
        IBridgeLocator bridgeLocator,
        IHueClient hueClient,
        ISettingsProvider settingsProvider)
    {
        _bridgeLocator = bridgeLocator;
        _hueClient = hueClient;
        _settingsProvider = settingsProvider;
    }

    public async Task ShowCapabilities()
    {
        var result = await _hueClient.GetCapabilitiesAsync();

        Console.WriteLine($"Available {nameof(result.Groups)} {result.Groups.Available} (total {result.Groups.Total})");
        Console.WriteLine($"Available {nameof(result.Lights)} {result.Lights.Available} (total {result.Lights.Total})");
        Console.WriteLine($"Available {nameof(result.Schedules)} {result.Scenes.Available} (total {result.Scenes.Total})");
        Console.WriteLine($"Available {nameof(result.Schedules)} {result.Schedules.Available} (total {result.Schedules.Total})");
        Console.WriteLine($"Available {nameof(result.Rules)} {result.Rules.Available} (total {result.Rules.Total})");
        Console.WriteLine($"Available {nameof(result.Resourcelinks)} {result.Resourcelinks.Available} (total {result.Resourcelinks.Total})");
        Console.WriteLine($"Available {nameof(result.Sensors)} {result.Sensors.Available} (total {result.Sensors.Total})");
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
