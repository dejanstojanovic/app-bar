using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopBar.Plugins.SystemResources
{
    internal class Configuration
    {
        public bool ShowCPU { get; set; }
        public bool ShowRAM { get; set; }

        public Configuration()
        {
            ShowCPU = true;
            ShowRAM = true;
        }
    }
}
