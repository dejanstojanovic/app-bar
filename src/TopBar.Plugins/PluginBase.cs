using System.Reflection;

namespace TopBar.Plugins
{
    public abstract class PluginBase : Panel, IPlugin
    {
        public abstract event EventHandler ApplicationExit;
        public abstract event EventHandler ApplicationRestart;

        public abstract string Name { get; }
        public abstract IEnumerable<ToolStripMenuItem> MenuItems { get; }

        string _configPath = null;
        public String ConfigurationFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_configPath))
                    _configPath = Path.Combine(
                               Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                               $"{this.GetType().Assembly.GetName().Name}.json");

                return _configPath;
            }
        }

        public abstract Task SaveConfig();

    }
}
