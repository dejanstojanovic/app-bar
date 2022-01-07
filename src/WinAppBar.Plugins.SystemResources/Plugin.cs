using System.Diagnostics;
using Forms = System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace WinAppBar.Plugins.SystemResources
{
    public class Plugin : PluginBase
    {
        public override event EventHandler ApplicationExit=null;
        public override event EventHandler ApplicationRestart = null;

        readonly ContextMenuStrip _contextMenuStripMain;
        readonly ColorTheme _colorTheme;
        readonly CpuUsage _cpuUsage;
        readonly RamUsage _ramUsage;

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

            //this.BackColor = Color.Black;

            _contextMenuStripMain = new ContextMenuStrip()
            {
                RenderMode = ToolStripRenderMode.System,
            };
            _contextMenuStripMain.Items.Add(new ToolStripMenuItem("Resource Monitor", null,
                (sender, e) =>
                    {
                        var windowsFolder = Environment.GetEnvironmentVariable("windir");
                        Process p = new Process();
                        p.StartInfo.WorkingDirectory = Path.Combine(windowsFolder, "System32");
                        p.StartInfo.FileName = Path.Combine(windowsFolder, "System32", "mmc.exe");
                        p.StartInfo.Arguments = Path.Combine(windowsFolder, "System32", "perfmon.msc") + " /s";
                        p.StartInfo.UseShellExecute = true;
                        p.Start();
                    }, "ResourceMonitor"));

            _contextMenuStripMain.Items.Add(new ToolStripMenuItem("Task Manager", null,
                (sender, e) =>
                {
                    Process p = new Process();
                    p.StartInfo.FileName = "taskmgr";
                    p.StartInfo.UseShellExecute = true;
                    p.Start();
                }, "TaskManager"));

            _contextMenuStripMain.Items.Add("-");

            _contextMenuStripMain.Items.Add(new ToolStripMenuItem("Restart application", null,
                (sender, e) =>
                {
                    if (this.ApplicationRestart != null)
                        this.ApplicationRestart.Invoke(this, EventArgs.Empty);
                }, "Restart"));
            this.ContextMenuStrip = _contextMenuStripMain;

            _contextMenuStripMain.Items.Add("-");

            _contextMenuStripMain.Items.Add(new ToolStripMenuItem("Exit", null,
                (sender, e) =>
                {
                    if (this.ApplicationExit != null)
                        this.ApplicationExit.Invoke(this, EventArgs.Empty);
                }, "Exit"));
            this.ContextMenuStrip = _contextMenuStripMain;

            this.ContextMenuStrip = _contextMenuStripMain;



        }



       

        public override async Task SaveConfig()
        {
            await Task.CompletedTask;
        }
    }
}
