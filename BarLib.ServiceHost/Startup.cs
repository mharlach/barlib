using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(BarLib.ServiceHost.Startup))]

namespace BarLib.ServiceHost
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder){
            
            builder.Services.AddSingleton<IStorageContext<Ingredient>,ModelBaseStorageContext<Ingredient>>();
            builder.Services.AddSingleton<IStorageContext<Drink>,ModelBaseStorageContext<Drink>>();
            builder.Services.AddSingleton<IStorageContext<UserBar>,UserModelBaseStorageContext<UserBar>>();
            builder.Services.AddSingleton<IStorageContext<UserLibrary>,UserModelBaseStorageContext<UserLibrary>>();
            builder.Services.AddSingleton<ILibraryGenerator,BruteForceLibraryGenerator>();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }
    }
}