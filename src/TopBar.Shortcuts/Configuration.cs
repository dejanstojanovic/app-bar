using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopBar.Plugins.Shortcuts.Extensions;

namespace TopBar.Plugins.Shortcuts
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

        public ShortcutConfiguration()
        {

        }

        public ShortcutConfiguration(string path)
        {
            this.Path = Environment.ExpandEnvironmentVariables(path);
            if(path.IsDirectory())
                this.Label = System.IO.Path.GetDirectoryName(path);
            else
                this.Label = System.IO.Path.GetFileName(path);
        }

    }
}
