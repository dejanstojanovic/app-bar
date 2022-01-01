using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppBar.Plugins;
using WinAppBar.Shortcuts;

namespace WinAppBar
{
    public class PluginLoader:IPluginLoader
    {
        public PluginLoader()
        {

        }

        public IEnumerable<IPlugin> LoadPlugins(Form host)
        {
            var shortcuts = new Plugin();

            host.Controls.Add(shortcuts);

            return new[] { shortcuts };
        }
    }
}
