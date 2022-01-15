using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopBar.Plugins.Extensions;

namespace TopBar.Plugins.WorldClock
{
    internal class DateTimeControl : Panel
    {
        readonly ColorTheme _colorTheme;
        readonly Label _label;
        readonly PictureBox _pictureBox;
        readonly ClockConfiguration _configuration;
        public DateTimeControl(ColorTheme colorTheme, ClockConfiguration configuration) : base()
        {
            _configuration = configuration;
            _colorTheme = colorTheme;
            _label = new Label()
            {
                ForeColor = _colorTheme.TextColor,
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 0, 0, 0),
                Text = DateTime.Now.ToString($"Dubai, { CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern} {CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern}")
            };
            this.Controls.Add(_label);

            var iconColor = _colorTheme.TextColor.R == 0 && _colorTheme.TextColor.G == 0 && _colorTheme.TextColor.B == 0 ?
                           nameof(Color.Black).ToLower() :
                           nameof(Color.White).ToLower();
            _pictureBox = new PictureBox()
            {
                Width = 16,
                Image = Bitmap.FromStream(this.GetType().Assembly.GetManifestResourceStream($"{this.GetType().Namespace}.Icons.time_{iconColor}.png")),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Dock = DockStyle.Left
            };
            this.Controls.Add(_pictureBox);

            this.ResizeControl();
        }

        private void ResizeControl()
        {
            _label.Width = _label.MeasureDisplayStringWidth();
            this.Width = _pictureBox.Width + _label.Width;
        }
    }
}
