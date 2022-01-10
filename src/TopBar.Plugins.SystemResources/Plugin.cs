using System.Diagnostics;
using System.Text.Json;

namespace TopBar.Plugins.SystemResources
{
    public class Plugin : PluginBase
    {
        readonly ColorTheme _colorTheme;
        readonly CpuUsage _cpuUsage;
        readonly RamUsage _ramUsage;
        readonly IEnumerable<ToolStripMenuItem> _menuItems;

        readonly Configuration _configuration;

        public override IEnumerable<ToolStripMenuItem> MenuItems => _menuItems;

        public override string Name => "System resources";

        public Plugin(ColorTheme colorTheme) : base()
        {
            _colorTheme = colorTheme;

            _configuration = LoadConfiguration(typeof(Configuration)) as Configuration ?? new Configuration();

            _cpuUsage = new CpuUsage()
            {
                Dock = DockStyle.Right
            };

            _ramUsage = new RamUsage()
            {
                Dock = DockStyle.Right
            };

            if (_configuration.ShowCPU)
                _cpuUsage.Enable();
            else
                _cpuUsage.Disable();

            if (_configuration.ShowRAM)
                _ramUsage.Enable();
            else
                _ramUsage.Disable();

            this.Resize();
            
            this.Dock = DockStyle.Right;

            this.Controls.Add(_cpuUsage);
            this.Controls.Add(_ramUsage);

            _menuItems = new ToolStripMenuItem[] {
                new ToolStripMenuItem("CPU usage", null,
                (sender, e) =>
                    {
                        _configuration.ShowCPU=!_configuration.ShowCPU;

                        if(_configuration.ShowCPU)
                            _cpuUsage.Enable();
                        else
                            _cpuUsage.Disable();

                        this.Resize();
                        var item = sender as ToolStripMenuItem;
                        if(item != null)
                            item.Checked = _configuration.ShowCPU;

                    },"Cpu"){Checked = _configuration.ShowCPU},
                new ToolStripMenuItem("RAM usage", null,
                (sender, e) =>
                    {
                        _configuration.ShowRAM=!_configuration.ShowRAM;

                        if(_configuration.ShowRAM)
                            _ramUsage.Enable();
                        else
                            _ramUsage.Disable();

                        this.Resize();
                        var item = sender as ToolStripMenuItem;
                        if(item != null)
                            item.Checked = _configuration.ShowRAM;
                    },"Ram"){Checked = _configuration.ShowRAM},
            new ToolStripMenuItem("Open Resource Monitor", null,
                (sender, e) =>
                    {
                        var windowsFolder = Environment.GetEnvironmentVariable("windir");
                        Process p = new Process();
                        p.StartInfo.WorkingDirectory = Path.Combine(windowsFolder, "System32");
                        p.StartInfo.FileName = Path.Combine(windowsFolder, "System32", "mmc.exe");
                        p.StartInfo.Arguments = Path.Combine(windowsFolder, "System32", "perfmon.msc") + " /s";
                        p.StartInfo.UseShellExecute = true;
                        p.Start();
                    }, "ResourceMonitor"),
            new ToolStripMenuItem("Open Task Manager", null,
                (sender, e) =>
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = "taskmgr";
                        p.StartInfo.UseShellExecute = true;
                        p.Start();
                    }, "TaskManager")
            };

        }

        private void Resize()
        {
            this.Width = (_cpuUsage.Enabled ? _cpuUsage.Width :0) +
                (_ramUsage.Enabled ? _ramUsage.Width : 0);
        }

        public override async Task SaveConfiguration()
        {
            var configuration = new Configuration()
            {
                ShowCPU = _cpuUsage.Visible,
                ShowRAM = _ramUsage.Visible
            };

            var configContent = JsonSerializer.Serialize<Configuration>(
                 configuration,
                 new JsonSerializerOptions()
                 {
                     WriteIndented = true
                 });

            await File.WriteAllTextAsync(path: this.ConfigurationFilePath, configContent);
        }
    }
}
