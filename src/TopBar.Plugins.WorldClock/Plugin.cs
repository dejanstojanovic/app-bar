using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopBar.Plugins.WorldClock
{
    public class Plugin : PluginBase
    {
        public override string Name => "World Clock";

        public override IEnumerable<ToolStripMenuItem> MenuItems => null;

        public override async Task SaveConfiguration()
        {
            throw new NotImplementedException();
        }
    }
}
