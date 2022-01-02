using System.Diagnostics;
using Forms = System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace WinAppBar.Plugins.SystemResources
{
    public class Plugin : Panel, IPlugin
    {
        public event EventHandler ApplicationExit;

        readonly Label label;
        readonly Forms.Timer timer;
        readonly PerformanceCounter cpuCounter;
        readonly ContextMenuStrip contextMenuStripMain;

        public Plugin() : base()
        {
            this.Width = 90;
            this.Dock = DockStyle.Right;

            label = new Label();
            label.Dock = DockStyle.Fill;
            label.ForeColor = ColorUtils.GetTextColor();

            label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            label.Padding = new Padding(0, 0, 5, 0);
            label.Text = $"CPU: %";

            //label.MouseEnter += Label_MouseEnter;
            //label.MouseLeave += Label_MouseLeave;
            //label.Click += Label_Click;

            this.Controls.Add(label);

            contextMenuStripMain = new ContextMenuStrip()
            {
                RenderMode = ToolStripRenderMode.System,
            };
            contextMenuStripMain.Items.Add(new ToolStripMenuItem("Resource Monitor", null,
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

            contextMenuStripMain.Items.Add(new ToolStripMenuItem("Task Manager", null,
                (sender, e) =>
                {
                    Process p = new Process();
                    p.StartInfo.FileName = "taskmgr";
                    p.StartInfo.UseShellExecute = true;
                    p.Start();
                }, "TaskManager"));

            contextMenuStripMain.Items.Add("-");

            contextMenuStripMain.Items.Add(new ToolStripMenuItem("Exit", null,
                (sender, e) =>
                {
                    if (this.ApplicationExit != null)
                        this.ApplicationExit.Invoke(this, EventArgs.Empty);
                }, "Exit"));



            this.ContextMenuStrip = contextMenuStripMain;


            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            cpuCounter.ReadOnly = true;
            timer = new Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            Timer_Tick(timer, new EventArgs());
            timer.Start();
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            var cpu = await Task.Run<double>(() =>
            {
                var unused = cpuCounter.NextValue(); // first call will always return 0
                Thread.Sleep(750); // wait then try again
                unused = cpuCounter.NextValue();
                return Math.Round(unused, 2);
            });
            this.label.Text = $"CPU: {cpu}%";
        }

        private void Label_Click(object? sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            if (args != null && args.Button == MouseButtons.Right)
                return;


        }

        private void Label_MouseLeave(object? sender, EventArgs e)
        {
            var control = sender as Label;
            if (control != null)
                control.BackColor = Color.Transparent;
        }

        private void Label_MouseEnter(object? sender, EventArgs e)
        {
            var accentColor = ColorUtils.GetAccentColor();
            var control = sender as Label;
            if (control != null)
                control.BackColor = accentColor;
        }

        public async Task SaveConfig()
        {
            await Task.CompletedTask;
        }
    }
}
