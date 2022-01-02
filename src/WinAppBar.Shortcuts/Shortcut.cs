using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppBar.Plugins.Shortcuts
{
    internal class Shortcut : Panel
    {
        public string Label { get => _label.Text; set => _label.Text = value; }
        public Image Icon { get => _pictureBox.Image; set => _pictureBox.Image = value; }

        private Label _label;
        private PictureBox _pictureBox;
        public Shortcut(string path)
        {
            this.BackColor = Color.Orange;

            _pictureBox = new PictureBox()
            {
                Size = new Size(24, 28),
                Padding = new Padding(4),
                Top = 5,
                Left = 0,
                Tag = path
            };
            _pictureBox.Left = 5;

            _pictureBox.BackColor = Color.White;

            this.Controls.Add(_pictureBox);

            _label = new Label();
            _label.AutoSize = true;
            _label.TextAlign = ContentAlignment.MiddleLeft;
            _label.ForeColor = ColorUtils.GetTextColor();
            _label.Top = _pictureBox.Top;
            _label.Height = _pictureBox.Height +
                            _pictureBox.Padding.Top +
                            _pictureBox.Padding.Bottom;
            _label.BackColor = Color.Blue;
            _label.Left = _pictureBox.Left +
                          //_pictureBox.Padding.Left +
                          _pictureBox.Width;
            //_pictureBox.Padding.Right;
            _label.TextChanged += _label_TextChanged;

            this.Controls.Add(_label);
        }

        void Resize()
        {
            this.Height = _pictureBox.Height;

            var width = this.Width = //_pictureBox.Left +
                              _pictureBox.Padding.Left +
                              _pictureBox.Width +
                              _pictureBox.Padding.Right;

            if (_label.Visible)
            {
                SizeF labelSize = _label.CreateGraphics().MeasureString(_label.Text, _label.Font);
                width = width + (int)labelSize.Width;
            }

            this.Width = width;
        }

        private void _label_TextChanged(object? sender, EventArgs e)
        {
            this.Resize();
        }
    }
}
