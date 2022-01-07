using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TopBar.Plugins;

namespace TopBar
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool canCreateInstance = false;
            var mutex = new Mutex(true, "330ECF4F-5DB6-4A70-B227-D03EB2AC9E6F", out canCreateInstance);

            if (canCreateInstance)
            {
                ApplicationConfiguration.Initialize();

                var services = new ServiceCollection();

                ConfigureServices(services);

                using (ServiceProvider serviceProvider = services.BuildServiceProvider())
                {
                    var mainForm = serviceProvider.GetRequiredService<MainForm>();
                    Application.Run(mainForm);
                }
                mutex.ReleaseMutex();
            }
            else
                return;
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddScoped<MainForm>();
            
            services.AddScoped<IPlugin, Plugins.Shortcuts.Plugin>();
            services.AddScoped<IPlugin, Plugins.SystemResources.Plugin>();

            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
                IConfiguration config = configBuilder.Build();

            services.AddSingleton<IConfiguration>(config);

            services.AddSingleton<ColorTheme>(new ColorTheme()
            {
                BackgroudColor = ColorTranslator.FromHtml(config.GetValue<String>($"{nameof(ColorTheme)}:{nameof(ColorTheme.BackgroudColor)}")),
                TextColor = ColorTranslator.FromHtml(config.GetValue<String>($"{nameof(ColorTheme)}:{nameof(ColorTheme.TextColor)}")),
                HoverColor = ColorTranslator.FromHtml(config.GetValue<String>($"{nameof(ColorTheme)}:{nameof(ColorTheme.HoverColor)}"))
            });


        }
    }
}