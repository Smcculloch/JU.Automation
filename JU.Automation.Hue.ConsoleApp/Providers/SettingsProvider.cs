using Microsoft.Extensions.Configuration;

namespace JU.Automation.Hue.ConsoleApp.Providers
{
    public interface ISettingsProvider
    {
        string LocalHueClientIp { get; }
        string AppKey { get; }
        bool EnableDebug { get; }
        string Wakeup1SensorUniqueId { get; }
    }

    public class SettingsProvider : ISettingsProvider
    {
        private const string HueIpCommandLineArg = "hue-ip";
        private const string HueAppKeyCommandLineArg = "hue-appkey";
        private const string EnableDebugCommandLineArg = "enable-debug";
        private const string Wakeup1SensorUniqueIdKey = "Client:Wakeup1SensorUniqueId";

        private readonly IConfiguration _configuration;

        public SettingsProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string LocalHueClientIp => _configuration[HueIpCommandLineArg];
        public string AppKey => _configuration[HueAppKeyCommandLineArg] ?? string.Empty;
        public bool EnableDebug => bool.Parse(_configuration[EnableDebugCommandLineArg] ?? bool.FalseString);
        public string Wakeup1SensorUniqueId => _configuration[Wakeup1SensorUniqueIdKey];
    }
}
