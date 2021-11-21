using System;
using System.IO;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Actions.Setup;
using JU.Automation.Hue.ConsoleApp.Automations;
using JU.Automation.Hue.ConsoleApp.Providers;
using JU.Automation.Hue.ConsoleApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                   .ConfigureLogging(logging =>
                   {
                       logging.ClearProviders();
                       logging.SetMinimumLevel(LogLevel.Trace).AddNLog();
                   })
                   .ConfigureServices(services =>
                   {
                       services.AddHostedService<ScopedBackgroundService>();
                       services.AddScoped<HueSetupApplication>();

                       var configuration = new ConfigurationBuilder()
                                           .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                                           .AddJsonFile("appsettings.json", false)
                                           .AddCommandLine(args)
                                           .Build();

                       services.AddSingleton<IConfiguration>(configuration);
                       services.AddTransient<ISettingsProvider, SettingsProvider>();

                       services.AddHttpClient<IHueClient, HueClient>();
                       services.AddTransient<IBridgeLocator, HttpBridgeLocator>();

                       services.AddTransient<ISetupAction, SetupActionStep1RenameLights>();
                       services.AddTransient<ISetupAction, SetupActionStep2GroupLights>();

                       services.AddTransient<IWakeupAutomationSetupAction<Automations.Wakeup.WakeupModel>, Automations.Wakeup.ActionStep1CreateSensors>();
                       services.AddTransient<IWakeupAutomationSetupAction<Automations.Wakeup.WakeupModel>, Automations.Wakeup.ActionStep2CreateScenes>();
                       services.AddTransient<IWakeupAutomationSetupAction<Automations.Wakeup.WakeupModel>, Automations.Wakeup.ActionStep3CreateSchedules>();
                       services.AddTransient<IWakeupAutomationSetupAction<Automations.Wakeup.WakeupModel>, Automations.Wakeup.ActionStep4CreateRules>();

                       services.AddTransient<ISunriseAutomationSetupAction<Automations.Sunrise.SunriseModel>, Automations.Sunrise.ActionStep1CreateSensors>();
                       services.AddTransient<ISunriseAutomationSetupAction<Automations.Sunrise.SunriseModel>, Automations.Sunrise.ActionStep2CreateScenes>();
                       services.AddTransient<ISunriseAutomationSetupAction<Automations.Sunrise.SunriseModel>, Automations.Sunrise.ActionStep3CreateSchedules>();
                       services.AddTransient<ISunriseAutomationSetupAction<Automations.Sunrise.SunriseModel>, Automations.Sunrise.ActionStep4CreateRules>();

                       services.AddTransient<IAutomationActionService, AutomationActionService>();
                       services.AddTransient<IGenericActionService, GenericActionService>();
                       services.AddTransient<ISetupActionService, SetupActionService>();
                       services.AddTransient<IResetActionService, ResetActionService>();
                       services.AddTransient<IUserInputService, UserInputService>();

                       services.AddScoped<IHueSetupCoordinator, HueSetupCoordinator>();
                   });
    }
}
