using System;
using System.IO;
using System.Linq;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Actions;
using JU.Automation.Hue.ConsoleApp.Actions.AutomationReset;
using JU.Automation.Hue.ConsoleApp.Actions.AutomationSetup;
using JU.Automation.Hue.ConsoleApp.Actions.Reset;
using JU.Automation.Hue.ConsoleApp.Actions.Setup;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
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

                       services.AddTransient<ISetupAction, SetupActionStep1NewDeveloper>();
                       services.AddTransient<ISetupAction, SetupActionStep2SearchNewLights>();
                       services.AddTransient<ISetupAction, SetupActionStep3RenameLights>();
                       services.AddTransient<ISetupAction, SetupActionStep4GroupLights>();

                       services.AddTransient<IAutomationSetupAction, AutomationSetupActionStep1CreateSensors>();
                       services.AddTransient<IAutomationSetupAction, AutomationSetupActionStep2CreateScenes>();
                       services.AddTransient<IAutomationSetupAction, AutomationSetupActionStep3CreateSchedules>();
                       services.AddTransient<IAutomationSetupAction, AutomationSetupActionStep4CreateRules>();

                       services.AddTransient<IAutomationResetAction, AutomationResetActionStep96DeleteRules>();
                       services.AddTransient<IAutomationResetAction, AutomationResetActionStep97DeleteSchedules>();
                       services.AddTransient<IAutomationResetAction, AutomationResetActionStep98DeleteScenes>();
                       services.AddTransient<IAutomationResetAction, AutomationResetActionStep99DeleteSensors>();

                       services.AddTransient<IResetAction, ResetActionStep98DeleteGroups>();
                       services.AddTransient<IResetAction, ResetActionStep99DeleteLights>();

                       services.AddScoped<IHueConfigService, HueConfigService>(x =>
                       {
                           var setupActionSteps = x.GetServices<ISetupAction>()
                                                   .Cast<IStep>()
                                                   .OrderBy(actionStep => actionStep.Step)
                                                   .Cast<ISetupAction>();

                           var automationSetupActionSteps = x.GetServices<IAutomationSetupAction>()
                                                             .Cast<IStep>()
                                                             .OrderBy(actionStep => actionStep.Step)
                                                             .Cast<IAutomationSetupAction>();

                           var resetActionSteps = x.GetServices<IResetAction>()
                                                   .Cast<IStep>()
                                                   .OrderBy(actionStep => actionStep.Step)
                                                   .Cast<IResetAction>();

                           var automationResetActionSteps = x.GetServices<IAutomationResetAction>()
                                                             .Cast<IStep>()
                                                             .OrderBy(actionStep => actionStep.Step)
                                                             .Cast<IAutomationResetAction>();

                           return new HueConfigService(
                               setupActionSteps,
                               automationSetupActionSteps,
                               resetActionSteps,
                               automationResetActionSteps);
                       });
                   });
    }
}
