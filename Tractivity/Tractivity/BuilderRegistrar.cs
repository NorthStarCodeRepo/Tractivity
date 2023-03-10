using Tractivity.AppServices;
using Tractivity.Common.Environment;
using Tractivity.Managers;
using Tractivity.Views;

namespace Tractivity
{
    public static class BuilderRegistrar
    {
        public static void RegisterDependencies(this MauiAppBuilder builder)
        {
            // Register DI
            builder.Services.AddSingleton<EnvironmentManager>();
            builder.Services.AddTransient<ILocationManagerFactory, LocationManagerFactory>();
            builder.Services.AddTransient<ILocationService, LocationService>();
            builder.Services.AddSingleton<WalkingService>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<WalkingView>();
            builder.Services.AddTransient<ExportLogsView>();
        }
    }
}