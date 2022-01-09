using System.Diagnostics;

namespace TopBar.Plugins.SystemResources
{
    public class Plugin : PluginBase
    {

        public override event EventHandler ApplicationExit=null;
        public override event EventHandler ApplicationRestart = null;

        readonly ColorTheme _colorTheme;
        readonly CpuUsage _cpuUsage;
        readonly RamUsage _ramUsage;
        readonly IEnumerable<ToolStripMenuItem> _menuItems;

        public override IEnumerable<ToolStripMenuItem> MenuItems => _menuItems;

        public override string Name => "System resources";

        public Plugin() : base()
        {
            _colorTheme = new ColorTheme();

            _cpuUsage = new CpuUsage()
            {
                Dock = DockStyle.Right
            };

            _ramUsage = new RamUsage()
            {
                Dock = DockStyle.Right
            };

            this.Width = _cpuUsage.Width + _ramUsage.Width;
            this.Dock = DockStyle.Right;

            this.Controls.Add(_cpuUsage);
            this.Controls.Add(_ramUsage);


            _menuItems = new ToolStripMenuItem[] {new ToolStripMenuItem("Resource Monitor", null,
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
            new ToolStripMenuItem("Task Manager", null,
                (sender, e) =>
                {
                    Process p = new Process();
                    p.StartInfo.FileName = "taskmgr";
                    p.StartInfo.UseShellExecute = true;
                    p.Start();
                }, "TaskManager")
            };

        }

        public override async Task SaveConfig()
        {
            await Task.CompletedTask;
        }
    }
}
