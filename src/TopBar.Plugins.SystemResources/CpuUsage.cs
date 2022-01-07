using System.Diagnostics;
using Forms = System.Windows.Forms;

namespace TopBar.Plugins.SystemResources
{
    internal class CpuUsage : Panel
    {
        readonly ToolTip _toolTip;
        readonly Label _label;
        readonly PictureBox _pictureBox;
        readonly Forms.Timer _timer;
        readonly PerformanceCounter _cpuCounter;
        readonly ColorTheme _colorTheme;

        public CpuUsage() : base()
        {
            _colorTheme = new ColorTheme();

            _toolTip = new ToolTip() {
                AutoPopDelay = 0,
                InitialDelay = 1,
                ReshowDelay = 0,
                ShowAlways = true 
            };

            this.Width = 75;
            this.Padding = new Padding(5, 0, 5, 0);

            _label = new Label()
            {
                Dock = DockStyle.Right,
                ForeColor = _colorTheme.TextColor,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 0, 0, 0),
                Text = $"---.--",
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

            foreach (Control control in this.Controls)
                control.MouseHover += Control_MouseHover;

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

        private void Control_MouseHover(object? sender, EventArgs e)
        {
            var shortcut = sender as Control;
            if (shortcut != null)
            {
                _toolTip.SetToolTip(shortcut, "CPU");
            }
        }
    }
}
