using System.Diagnostics;
using Forms = System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace WinAppBar.Plugins.SystemResources
{
    public class Plugin : PluginBase
    {
        public override event EventHandler ApplicationExit;

        readonly Label _label;
        readonly PictureBox _pictureBox;
        readonly Forms.Timer _timer;
        readonly PerformanceCounter _cpuCounter;
        readonly ContextMenuStrip _contextMenuStripMain;
        readonly ColorTheme _colorTheme;

        public Plugin() : base()
        {
            _colorTheme = new ColorTheme();

            this.Width = 70;
            this.Dock = DockStyle.Right;

            _label = new Label()
            {
                Dock = DockStyle.Right,
                ForeColor = _colorTheme.TextColor,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 0, 5, 0),
                Text = $"-.-%",
                Width = 55
            };

            this.Controls.Add(_label);

            _pictureBox = new PictureBox()
            {
                Width = 16,
                Image = Bitmap.FromStream(this.GetType().Assembly.GetManifestResourceStream($"{this.GetType().Namespace}.cpu.png")),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Dock = DockStyle.Left
            };

            this.Controls.Add(_pictureBox);
            _pictureBox.BringToFront();

            //label.MouseEnter += Label_MouseEnter;
            //label.MouseLeave += Label_MouseLeave;
            //label.Click += Label_Click;

            _cpuCounter = new PerformanceCounter()
            {
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total",
                ReadOnly = true
            };

            _timer = new Forms.Timer()
            {
                Interval = 1000
            };

            _timer.Tick += Timer_Tick;
            _timer.Start();
        

            Timer_Tick(_timer, new EventArgs());

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

            _contextMenuStripMain.Items.Add(new ToolStripMenuItem("Exit", null,
                (sender, e) =>
                {
                    if (this.ApplicationExit != null)
                        this.ApplicationExit.Invoke(this, EventArgs.Empty);
                }, "Exit"));

            this.ContextMenuStrip = _contextMenuStripMain;



        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            var cpu = await Task.Run<double>(() =>
            {
                var total = _cpuCounter.NextValue(); // first call will always return 0
                Thread.Sleep(750); // wait then try again
                total = _cpuCounter.NextValue();
                return Math.Round(total, 2);
            });
            this._label.Text = $"{cpu}%";
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
            var control = sender as Label;
            if (control != null)
                control.BackColor = _colorTheme.HoverColor;
        }

        public override async Task SaveConfig()
        {
            await Task.CompletedTask;
        }
    }
}
