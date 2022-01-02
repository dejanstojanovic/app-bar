using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppBar.Plugins
{
    public interface IPlugin
    {
        event EventHandler ApplicationExit;
        Task SaveConfig();
    }
}
