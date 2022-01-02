using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppBar.Plugins;

namespace WinAppBar
{
    internal interface IPluginLoader
    {
        IEnumerable<IPlugin> LoadPlugins(Form host);
        Task SavePlugins(Form host);
    }
}
