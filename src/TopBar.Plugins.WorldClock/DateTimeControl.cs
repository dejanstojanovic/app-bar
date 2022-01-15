using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopBar.Plugins.Extensions;
using Forms = System.Windows.Forms;

namespace TopBar.Plugins.WorldClock
{
    internal class DateTimeControl : Panel
    {
        readonly ColorTheme _colorTheme;
        readonly Label _label;
        readonly PictureBox _pictureBox;
        readonly ClockConfiguration _configuration;
        readonly Forms.Timer _timer;

        public DateTimeControl(ColorTheme colorTheme, ClockConfiguration configuration) : base()
        {
            _configuration = configuration;
            _colorTheme = colorTheme;

            this.BackColor = Color.Blue;

            _label = new Label()
            {
                BackColor = Color.Red,
                Height = 28,
                Width = 100,

                //ForeColor = _colorTheme.TextColor,
                ForeColor = Color.White,
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 0, 0, 0),
                //Text = DateTime.Now.ToString($"Dubai, { CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern} {CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern}")
                //Text ="Test"
            };
            this.Controls.Add(_label);

            

            var iconColor = _colorTheme.TextColor.R == 0 && _colorTheme.TextColor.G == 0 && _colorTheme.TextColor.B == 0 ?
                           nameof(Color.Black).ToLower() :
                           nameof(Color.White).ToLower();
            _pictureBox = new PictureBox()
            {
                Width = 16,
                Padding= new Padding(4,0,4,0),
                Image = Bitmap.FromStream(this.GetType().Assembly.GetManifestResourceStream($"{this.GetType().Namespace}.Icons.time_{iconColor}.png")),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Dock = DockStyle.Left
            };
            this.Controls.Add(_pictureBox);


            _label.Text = "BBBB";
            this.ResizeControl();

            _timer = new Forms.Timer() 
            {
                Interval = 1000
            };

            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _label.Text = DateTime.Now.ToString();
        }

        private void ResizeControl()
        {
            _label.Width = _label.MeasureDisplayStringWidth() + 48;
            this.Width = _pictureBox.Width + _label.Width;
        }
    }
}
