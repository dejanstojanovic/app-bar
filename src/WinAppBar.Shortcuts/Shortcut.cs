using System.Diagnostics;
using WinAppBar.Plugins.Shortcuts.Extensions;
using System.Linq;

namespace WinAppBar.Plugins.Shortcuts
{
    internal class Shortcut : Panel
    {
        public string Label
        {
            get => _label.Text;
            set
            {
                _label.Text = value;
                SetSizes();
            }
        }
        public Image Icon { get => _pictureBox.Image; set => _pictureBox.Image = value; }
        

        readonly ToolTip toolTip;
        readonly Label _label;
        readonly PictureBox _pictureBox;

        public Shortcut(string path)
        {
            toolTip = new ToolTip()
            {
                AutoPopDelay = 0,
                InitialDelay = 1,
                ReshowDelay = 0,
                ShowAlways = true
            };

            //this.BackColor = Color.Orange;

            _pictureBox = new PictureBox()
            {
                Size = new Size(24, 24),
                Padding = new Padding(4),
                Top = 0,
                Left = 0,
                Tag = path
            };
            _pictureBox.Left = 0;

            //_pictureBox.BackColor = Color.White;

            this.Controls.Add(_pictureBox);

            _label = new Label();
            _label.TextAlign = ContentAlignment.MiddleLeft;
            _label.ForeColor = ColorUtils.GetTextColor();
            _label.Top = _pictureBox.Top;
            _label.AutoSize = true;
            _label.Height = _pictureBox.Height +
                            _pictureBox.Padding.Top +
                            _pictureBox.Padding.Bottom;
            //_label.BackColor = Color.Blue;
            _label.Left = _pictureBox.Left + _pictureBox.Width;
            _label.TextChanged += _label_TextChanged;


            this.Controls.Add(_label);


            foreach (Control control in this.Controls)
            {
                control.MouseEnter += Shortcut_MouseEnter;
                control.MouseLeave += Shortcut_MouseLeave;
                control.Click += Shortcut_Click;
                control.MouseHover += Shortcut_MouseHover;
            }

            this.MouseEnter += Shortcut_MouseEnter;
            this.MouseLeave += Shortcut_MouseLeave;
            this.Click += Shortcut_Click;
            this.MouseHover += Shortcut_MouseHover;

        }

        public void ShowLabel()
        {
            this._label.Visible = true;
            this.SetSizes();
        }
        public void HideLabel()
        {
            this._label.Visible = false;
            this.SetSizes();
        }
        public void OpenShortcut()
        {
            var path = this.Tag != null ? this.Tag.ToString() : null;
            if (string.IsNullOrEmpty(path))
                return;
            var processInfo = new ProcessStartInfo();
            var executables = new String[] { ".exe", ".com", ".bat" };
            if (path.IsDirectory() ||
                !executables.Any(e => e.Equals(Path.GetExtension(path), StringComparison.InvariantCultureIgnoreCase)))
            {
                processInfo.FileName = path;
                processInfo.UseShellExecute = true;
            }
            else
            {
                processInfo.FileName = path;
                processInfo.UseShellExecute = false;
            }

            Process.Start(processInfo);

            this.Shortcut_MouseLeave(this, null);
        }

        public void UnHiglight()
        {
            if (this.BackColor != Color.Transparent ||
                this.Controls.Cast<Control>().Any(c => c.BackColor != Color.Transparent))
            {
                foreach (Control control in this.Controls)
                    control.BackColor = Color.Transparent;

                this.BackColor = Color.Transparent;
            }
        }

        private void SetSizes()
        {
            this.Height = _pictureBox.Height;
            var width = _pictureBox.Width;

            if (_label.Visible)
            {
                SizeF labelSize = _label.CreateGraphics().MeasureString(_label.Text, _label.Font);

                width = width + MeasureDisplayStringWidth(_label.CreateGraphics(), _label.Text, _label.Font);
                _label.Top = 1 + (_pictureBox.Height - (int)labelSize.Height) / 2;
                width = width + _pictureBox.Padding.Left;
            }
            this.Width = width;
        }

        public void ToggleLabel()
        {
            _label.Visible = !_label.Visible;
            SetSizes();
        }

        public static int MeasureDisplayStringWidth(Graphics graphics, string text,
                                            Font font)
        {
            StringFormat format = new StringFormat();
            RectangleF rect = new System.Drawing.RectangleF(0, 0, 1000, 1000);
            CharacterRange[] ranges = {
                new CharacterRange(0, text.Length)
            };
            Region[] regions = new Region[1];

            format.SetMeasurableCharacterRanges(ranges);

            regions = graphics.MeasureCharacterRanges(text, font, rect, format);
            rect = regions[0].GetBounds(graphics);

            return (int)(rect.Right + 1.0f);
        }

        private void _label_TextChanged(object? sender, EventArgs e)
        {
            this.SetSizes();
        }

        private void Shortcut_MouseLeave(object? sender, EventArgs e)
        {
            if (this.ClientRectangle.Contains(this.PointToClient(Control.MousePosition)))
                return;
            else
            {
                UnHiglight();
            }

        }

        private void Shortcut_MouseEnter(object? sender, EventArgs e)
        {
            var accentColor = ColorUtils.GetAccentColor();
            foreach (Control control in this.Controls)
                control.BackColor = accentColor;

            this.BackColor = accentColor;

        }

        private void Shortcut_Click(object? sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            if (args != null && args.Button == MouseButtons.Right)
                return;

            OpenShortcut();
        }

        private void Shortcut_MouseHover(object? sender, EventArgs e)
        {
            var shortcut = sender as Control;
            if (shortcut != null && 
                this.Tag != null && 
                !string.IsNullOrWhiteSpace(this.Tag.ToString()))
            {
                toolTip.SetToolTip(shortcut, this.Tag.ToString());
            }
        }

    }
}