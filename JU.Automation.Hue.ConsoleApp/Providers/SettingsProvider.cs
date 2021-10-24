using System;
using Microsoft.Extensions.Configuration;

namespace JU.Automation.Hue.ConsoleApp.Providers
{
    public interface ISettingsProvider
    {
        string LocalHueClientIp { get; }
        string AppKey { get; }
        bool EnableDebug { get; }
        string Wakeup1SensorUniqueId { get; }
        int WakeupTransitionInMinutes { get; }
        int TurnOffTransitionInMinutes { get; }
        void SetAppKey(string appKey);
    }

    public class SettingsProvider : ISettingsProvider
    {
        private const string HueIpCommandLineArg = "hue-ip";
        private const string HueAppKeyCommandLineArg = "hue-appkey";
        private const string EnableDebugCommandLineArg = "enable-debug";
        private const string Wakeup1SensorUniqueIdKey = "Client:Wakeup1SensorUniqueId";
        private const string WakeupTransitionInMinutesKey = "Client:WakeupTransitionInMinutes";
        private const string TurnOffTransitionInMinutesKey = "Client:TurnOffTransitionInMinutes";

        private string _appKey;

        private readonly IConfiguration _configuration;

        public SettingsProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string LocalHueClientIp => _configuration[HueIpCommandLineArg];

        public string AppKey
        {
            get
            {
                if (string.IsNullOrEmpty(_appKey))
                {
                    _appKey = _configuration[HueAppKeyCommandLineArg] ?? string.Empty;
                }

                return _appKey;
            }
        }

        public bool EnableDebug => bool.Parse(_configuration[EnableDebugCommandLineArg] ?? bool.FalseString);
        public string Wakeup1SensorUniqueId => _configuration[Wakeup1SensorUniqueIdKey];
        public int WakeupTransitionInMinutes => _configuration.GetValue<int>(WakeupTransitionInMinutesKey);
        public int TurnOffTransitionInMinutes => _configuration.GetValue<int>(TurnOffTransitionInMinutesKey);

        public void SetAppKey(string appKey)
        {
            if (!string.IsNullOrEmpty(_appKey))
                throw new ArgumentException("AppKey already set!");

            _appKey = appKey;
        }
    }
}
