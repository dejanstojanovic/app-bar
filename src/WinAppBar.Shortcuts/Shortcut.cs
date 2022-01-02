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
                Resize();
            }
        }

        public Image Icon { get => _pictureBox.Image; set => _pictureBox.Image = value; }

        private Label _label;
        private PictureBox _pictureBox;

        public Shortcut(string path)
        {
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
        }

        private void Resize()
        {
            this.Height = _pictureBox.Height;
            var width = _pictureBox.Width;

            if (_label.Visible)
            {
                SizeF labelSize = _label.CreateGraphics().MeasureString(_label.Text, _label.Font);

                width = width + MeasureDisplayStringWidth(_label.CreateGraphics(), _label.Text, _label.Font);
                _label.Top = 1 + (_pictureBox.Height - (int)labelSize.Height) / 2;
            }
            this.Width = width + _pictureBox.Padding.Left;
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
            this.Resize();
        }

    }
}