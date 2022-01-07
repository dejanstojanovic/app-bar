using Microsoft.Extensions.DependencyInjection;
using WinAppBar.Plugins;

namespace WinAppBar
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
            services.AddSingleton<ColorTheme>();
        }
    }
}