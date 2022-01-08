using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopBar.Plugins.Shortcuts.Extensions
{
    public static class PathExtensions
    {
        public static bool IsDirectory(this string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return true;

            return false;

        }
    }
}
