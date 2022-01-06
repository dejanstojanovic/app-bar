using System.Diagnostics;
using Forms = System.Windows.Forms;

namespace WinAppBar.Plugins.SystemResources
{
    internal class RamUsage:Panel
    {
        readonly Label _label;
        readonly PictureBox _pictureBox;
        readonly Forms.Timer _timer;
        readonly PerformanceCounter _cpuCounter;
        readonly ColorTheme _colorTheme;

        public RamUsage():base()
        {
            _colorTheme = new ColorTheme();

            this.Width = 75;
            this.Padding = new Padding(5,0,5,0);

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
                Image = Bitmap.FromStream(this.GetType().Assembly.GetManifestResourceStream($"{this.GetType().Namespace}.ram.png")),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Dock = DockStyle.Left
            };

            this.Controls.Add(_pictureBox);
            _pictureBox.BringToFront();

            _cpuCounter = new PerformanceCounter()
            {
                CategoryName = "Memory",
                CounterName = "% Committed Bytes In Use",
                //InstanceName = "_Total",
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
    }
}
