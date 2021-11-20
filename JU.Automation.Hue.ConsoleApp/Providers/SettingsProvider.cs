using System;
using Microsoft.Extensions.Configuration;

namespace JU.Automation.Hue.ConsoleApp.Providers
{
    public interface ISettingsProvider
    {
        string LocalHueClientIp { get; }
        string AppKey { get; }
        bool EnableDebug { get; }
        int WakeupTransitionUpInMinutes { get; }
        int WakeupTransitionDownDelayInMinutes { get; }
        int WakeupTransitionDownInMinutes { get; }
        void SetAppKey(string appKey);
    }

    public class SettingsProvider : ISettingsProvider
    {
        private const string HueIpCommandLineArg = "hue-ip";
        private const string HueAppKeyCommandLineArg = "hue-appkey";
        private const string EnableDebugCommandLineArg = "enable-debug";
        private const string WakeupTransitionInMinutesKey = "Client:WakeupTransitionUpInMinutes";
        private const string WakeupTransitionDownDelayInMinutesKey = "Client:WakeupTransitionDownDelayInMinutes";
        private const string WakeupTransitionDownInMinutesKey = "Client:WakeupTransitionDownInMinutes";

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
        public int WakeupTransitionUpInMinutes => _configuration.GetValue<int>(WakeupTransitionInMinutesKey);
        public int WakeupTransitionDownDelayInMinutes => _configuration.GetValue<int>(WakeupTransitionDownDelayInMinutesKey);
        public int WakeupTransitionDownInMinutes => _configuration.GetValue<int>(WakeupTransitionDownInMinutesKey);

        public void SetAppKey(string appKey)
        {
            if (!string.IsNullOrEmpty(_appKey))
                throw new ArgumentException("AppKey already set!");

            _appKey = appKey;
        }
    }
}
