using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopBar.Plugins.SystemResources
{
    internal interface IResourcePlugin
    {
        void Enable();
        void Disable();
    }
}
