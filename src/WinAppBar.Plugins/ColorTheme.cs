namespace WinAppBar.Plugins
{
    public class ColorTheme
    {
        public Color BackgroudColor { get; set; }
        public Color TextColor { get; set; }
        public Color HoverColor { get; set; }

        public ColorTheme()
        {
            this.BackgroudColor = ColorTranslator.FromHtml("#1D1D1F");
            this.TextColor = ColorTranslator.FromHtml("#FFFFFF");
            this.HoverColor = ColorTranslator.FromHtml("#434D59");
        }
    }
}
