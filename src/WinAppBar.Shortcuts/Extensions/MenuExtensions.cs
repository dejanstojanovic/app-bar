using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppBar.Plugins.Shortcuts.Extensions
{
    public static class MenuExtensions
    {
        public static Control GetSourceControl(this ToolStripMenuItem menuItem)
        {
            var menu = menuItem.Owner as ContextMenuStrip;
            if (menu != null)
                return menu.SourceControl;

            return null;
        }
    }
}
