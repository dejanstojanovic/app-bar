using System.Diagnostics;
using TopBar.Plugins.Shortcuts.Extensions;
using System.Linq;
using TopBar.Plugins.Shortcuts.Interop;
using TopBar.Plugins.Extensions;

namespace TopBar.Plugins.Shortcuts
{
    internal class Shortcut : RoundedPanel
    {
        readonly ToolTip _toolTip;
        readonly Label _label;
        readonly PictureBox _pictureBox;
        readonly ColorTheme _colorTheme;
        readonly ShortcutConfiguration _shortcutConfiguration;

        public ShortcutConfiguration ShortcutConfiguration => _shortcutConfiguration;

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

        public Shortcut(ShortcutConfiguration config, ColorTheme colorTheme)
        {
            _shortcutConfiguration = config;
            _colorTheme = colorTheme;

            this.Radius = 3;
            this.Thickness = 0;
            this.Top = 4;

            _toolTip = new ToolTip()
            {
                AutoPopDelay = 0,
                InitialDelay = 1,
                ReshowDelay = 0,
                ShowAlways = true
            };

            _pictureBox = new PictureBox()
            {
                Size = new Size(24, 24),
                Padding = new Padding(4),
                Top = 0,
                Left = 0
            };

            this.Controls.Add(_pictureBox);

            _label = new Label()
            {
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = _colorTheme.TextColor,
                Top = _pictureBox.Top,
                AutoSize = true,
                Height = _pictureBox.Height +
                            _pictureBox.Padding.Top +
                            _pictureBox.Padding.Bottom,
                Left = _pictureBox.Left + _pictureBox.Width
            };

            _label.TextChanged += _label_TextChanged;
            this.Controls.Add(_label);

            Icon icon = null;
            var fileExtensionsWithIcons = new String[] { ".exe", ".lnk", ".ico" };
            if (config.Path.IsDirectory())
                icon = Icons.GetIconForFile(config.Path, ShellIconSize.SmallIcon);
            else
            {
                var extension = Path.GetExtension(config.Path);
                icon = fileExtensionsWithIcons.Any(e => e.Equals(extension)) ?
                    Icons.GetIconForFile(config.Path, ShellIconSize.SmallIcon) :
                    Icons.GetIconForExtension(extension, ShellIconSize.SmallIcon);
            }
            if (icon != null)
                this._pictureBox.Image = icon.ToBitmap();

            _label.Text = _shortcutConfiguration.Label;


            foreach (Control control in this.Controls.Cast<Control>().Concat(new Control[] { this }))
            {
                control.MouseEnter += Shortcut_MouseEnter;
                control.MouseLeave += Shortcut_MouseLeave;
                control.Click += Shortcut_Click;
                control.MouseHover += Shortcut_MouseHover;
            }

        }

        public bool LabelShown { get => this._label.Visible; }

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
            var path = _shortcutConfiguration.Path;
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

            try
            {
                Process.Start(processInfo);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                processInfo.UseShellExecute = true;
                processInfo.Verbs.Append("runas");
                Process.Start(processInfo);
            }

            this.Shortcut_MouseLeave(this, null);
        }

        public void UnHiglightAll()
        {
            if (this.BackColor != Color.Transparent ||
                this.Controls.Cast<Control>().Any(c => c.BackColor != Color.Transparent))
            {
                foreach (Control control in this.Controls.Cast<Control>().Concat(new Control[] { this }))
                {
                    control.BackColor = Color.Transparent;
                    control.ForeColor = _colorTheme.TextColor;
                }

            }
        }

        private void SetSizes()
        {
            this.Height = _pictureBox.Height;
            var width = _pictureBox.Width;

            if (_label.Visible)
            {
                SizeF labelSize = _label.CreateGraphics().MeasureString(_label.Text, _label.Font);

                width = width + _label.MeasureDisplayStringWidth();
                _label.Top = 1 + (_pictureBox.Height - (int)labelSize.Height) / 2;
                width = width + _pictureBox.Padding.Left;
            }
            this.Width = width;
            this.Invalidate();
        }

        public void ToggleLabel()
        {
            _label.Visible = !_label.Visible;
            SetSizes();
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
                UnHiglightAll();
            }

        }

        private void Shortcut_MouseEnter(object? sender, EventArgs e)
        {
            foreach (Control control in this.Controls.Cast<Control>().Concat(new Control[] { this }))
            {
                control.BackColor = _colorTheme.HoverBackgroundColor;
                control.ForeColor = _colorTheme.HoverTextColor;
            }

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
                this.ShortcutConfiguration !=null &&
                !string.IsNullOrWhiteSpace(this.ShortcutConfiguration.Path))
            {
                _toolTip.SetToolTip(shortcut, this.ShortcutConfiguration.Path);
            }
        }

    }
}