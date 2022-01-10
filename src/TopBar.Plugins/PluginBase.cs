using System.Reflection;
using System.Text.Json;

namespace TopBar.Plugins
{
    public abstract class PluginBase : Panel, IPlugin
    {
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

        public abstract Task SaveConfiguration();

        public object LoadConfiguration(Type type)
        {
            if (!string.IsNullOrWhiteSpace(this.ConfigurationFilePath))
            {
                string configurationContent = null;
                if (File.Exists(ConfigurationFilePath))
                    configurationContent = File.ReadAllText(ConfigurationFilePath);
                else
                {
                    var resourceStream = this.GetType().Assembly.GetManifestResourceStream($"{this.GetType().Namespace}.{this.GetType().Namespace}.json");
                    if(resourceStream != null)
                        configurationContent = new StreamReader(resourceStream).ReadToEnd();
                }
                if(!string.IsNullOrWhiteSpace(configurationContent))
                    return JsonSerializer.Deserialize(configurationContent,type);
                else
                    return null;
            }
            return null;
        }
    }
}
