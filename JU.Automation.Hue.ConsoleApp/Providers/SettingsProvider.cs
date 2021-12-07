using System;
using Microsoft.Extensions.Configuration;

namespace JU.Automation.Hue.ConsoleApp.Providers
{
    public interface ISettingsProvider
    {
        string LocalHueClientIp { get; }
        string AppKey { get; }
        bool DebugEnabled { get; }
        int WakeupTransitionUpInMinutes { get; }
        int WakeupTransitionDownDelayInMinutes { get; }
        int WakeupTransitionDownInMinutes { get; }
        int SunriseTransitionUpInMinutes { get; }
        int EveningLightsOnInMinutesBeforeBedtime { get; }
        int EveningLightsOnTransitionUpInMinutes { get; }
        int BedtimeTransitionDown1DelayInMinutes { get; }
        int BedtimeTransitionDown1InMinutes { get; }
        int BedtimeTransitionDown2DelayInMinutes { get; }
        int BedtimeTransitionDown2InMinutes { get; }
        void SetAppKey(string appKey);
    }

    public class SettingsProvider : ISettingsProvider
    {
        private const string HueIpCommandLineArg = "hue-ip";
        private const string HueAppKeyCommandLineArg = "hue-appkey";
        private const string DebugCommandLineArg = "debug";
        private const string WakeupTransitionInMinutesKey = "Client:WakeupTransitionUpInMinutes";
        private const string WakeupTransitionDownDelayInMinutesKey = "Client:WakeupTransitionDownDelayInMinutes";
        private const string WakeupTransitionDownInMinutesKey = "Client:WakeupTransitionDownInMinutes";
        private const string SunriseTransitionUpInMinutesKey = "Client:SunriseTransitionUpInMinutes";
        private const string EveningLightsOnInMinutesBeforeBedtimeKey = "Client:EveningLightsOnInMinutesBeforeBedtime";
        private const string EveningLightsOnTransitionUpInMinutesKey = "Client:EveningLightsOnTransitionUpInMinutes";
        private const string BedtimeTransitionDown1DelayInMinutesKey = "Client:BedtimeTransitionDown1DelayInMinutes";
        private const string BedtimeTransitionDown1InMinutesKey = "Client:BedtimeTransitionDown1InMinutes";
        private const string BedtimeTransitionDown2DelayInMinutesKey = "Client:BedtimeTransitionDown2DelayInMinutes";
        private const string BedtimeTransitionDown2InMinutesKey = "Client:BedtimeTransitionDown2InMinutes";

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

        public bool DebugEnabled => bool.Parse(_configuration[DebugCommandLineArg] ?? bool.FalseString);
        public int WakeupTransitionUpInMinutes => _configuration.GetValue<int>(WakeupTransitionInMinutesKey);
        public int WakeupTransitionDownDelayInMinutes => _configuration.GetValue<int>(WakeupTransitionDownDelayInMinutesKey);
        public int WakeupTransitionDownInMinutes => _configuration.GetValue<int>(WakeupTransitionDownInMinutesKey);
        public int SunriseTransitionUpInMinutes => _configuration.GetValue<int>(SunriseTransitionUpInMinutesKey);
        public int EveningLightsOnInMinutesBeforeBedtime => _configuration.GetValue<int>(EveningLightsOnInMinutesBeforeBedtimeKey);
        public int EveningLightsOnTransitionUpInMinutes => _configuration.GetValue<int>(EveningLightsOnTransitionUpInMinutesKey);
        public int BedtimeTransitionDown1DelayInMinutes => _configuration.GetValue<int>(BedtimeTransitionDown1DelayInMinutesKey);
        public int BedtimeTransitionDown1InMinutes => _configuration.GetValue<int>(BedtimeTransitionDown1InMinutesKey);
        public int BedtimeTransitionDown2DelayInMinutes => _configuration.GetValue<int>(BedtimeTransitionDown2DelayInMinutesKey);
        public int BedtimeTransitionDown2InMinutes => _configuration.GetValue<int>(BedtimeTransitionDown2InMinutesKey);

        public void SetAppKey(string appKey)
        {
            if (!string.IsNullOrEmpty(_appKey))
                throw new ArgumentException("AppKey already set!");

            _appKey = appKey;
        }
    }
}
