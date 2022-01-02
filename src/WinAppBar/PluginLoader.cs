using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppBar.Plugins;
using Shortcuts = WinAppBar.Plugins.Shortcuts;
using SystemResources = WinAppBar.Plugins.SystemResources;

namespace WinAppBar
{
    public class PluginLoader : IPluginLoader
    {
        public PluginLoader()
        {

        }

        public IEnumerable<IPlugin> LoadPlugins(Form host)
        {
            var plugins = new IPlugin[]
            {
                new Shortcuts.Plugin(),
                new SystemResources.Plugin(),
            };

            foreach (var plugin in plugins)
                host.Controls.Add(plugin as Control);

            return plugins;
        }

        public async Task SavePlugins(Form host)
        {
            foreach (IPlugin plugin in host.Controls)
                await plugin.SaveConfig();
        }
    }
}
