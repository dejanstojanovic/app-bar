using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppBar.Shortcuts;

namespace WinAppBar
{
    public class PluginLoader:IPluginLoader
    {
        public PluginLoader()
        {

        }

        public void LoadPlugins(Form host)
        {
            var shortcuts = new Plugin();
            host.Controls.Add(shortcuts);
        }

    }
}
