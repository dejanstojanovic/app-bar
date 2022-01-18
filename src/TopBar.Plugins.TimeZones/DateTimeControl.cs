using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopBar.Plugins.Extensions;
using Forms = System.Windows.Forms;

namespace TopBar.Plugins.TimeZones
{
    internal class DateTimeControl : Panel
    {
        readonly ColorTheme _colorTheme;
        readonly Label _label;
        //readonly PictureBox _pictureBox;
        readonly ClockConfiguration _configuration;
        readonly Forms.Timer _timer;

        public ClockConfiguration Configuration { get => _configuration; }

        private string GetText()
        {
            DateTime zoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _configuration.TimeZone);
            string zoneTimeText = zoneTime.ToString($"{ CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern} {CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern}");
            return $"{_configuration.Title}, {zoneTimeText}";
        }
        public DateTimeControl(ColorTheme colorTheme, ClockConfiguration configuration) : base()
        {
            _configuration = configuration;
            _colorTheme = colorTheme;

            _label = new Label()
            {
                //BackColor = Color.Red,
                ForeColor = _colorTheme.TextColor,
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 0, 0, 0),
                Text = GetText(),
            };
            this.Controls.Add(_label);

            var iconColor = _colorTheme.TextColor.R == 0 && _colorTheme.TextColor.G == 0 && _colorTheme.TextColor.B == 0 ?
                           nameof(Color.Black).ToLower() :
                           nameof(Color.White).ToLower();

            //_pictureBox = new PictureBox()
            //{
            //    //BackColor = Color.Orange,

            //    Width = 20,
            //    Image = Bitmap.FromStream(this.GetType().Assembly.GetManifestResourceStream($"{this.GetType().Namespace}.Icons.calendar_{iconColor}.png")),
            //    SizeMode = PictureBoxSizeMode.CenterImage,
            //    Dock = DockStyle.Left,
            //};

            //this.Controls.Add(_pictureBox);
            //_pictureBox.BringToFront();

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
            _label.Text = GetText();
            ResizeControl();
        }

        private void ResizeControl()
        {
            _label.Width = _label.MeasureDisplayStringWidth();
            //this.Width = _pictureBox.Width + _pictureBox.Padding.Left + _pictureBox.Padding.Right + _label.Width;
            this.Width= _label.Width;

        }
    }
}
