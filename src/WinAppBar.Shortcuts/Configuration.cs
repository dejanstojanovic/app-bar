using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppBar.Plugins.Shortcuts
{
    internal class Configuration
    {
        public bool ShowLabels { get; set; }
        public IEnumerable<ShortcutConfiguration> Shortcuts { get; set; }

        public Configuration()
        {
            this.ShowLabels = true;
        }
    }

    class ShortcutConfiguration
    {
        public string Path { get; set; }
        public string Label { get; set; }
    }
}
