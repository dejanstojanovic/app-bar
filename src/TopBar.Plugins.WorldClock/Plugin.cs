using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopBar.Plugins.WorldClock
{
    public class Plugin : PluginBase
    {
        readonly ColorTheme _colorTheme;
        readonly IEnumerable<ToolStripMenuItem> _menuItems;
        public override string Name => "World Clock";
        public override IEnumerable<ToolStripMenuItem> MenuItems => _menuItems;

        public Plugin(ColorTheme colorTheme) : base()
        {
            _menuItems = new ToolStripMenuItem[] {
                new ToolStripMenuItem("Settings...", null,
                (sender, e) =>
                    {

                    },"Settings")
                };


            this.BackColor = Color.Green;


            _colorTheme = colorTheme;
            var clockControl = new DateTimeControl(_colorTheme, new ClockConfiguration()
            {
                Active = true,
                TimeZoneId = "Dubai GMT+04",
                Title = "UAE"
            });
            this.Controls.Add(clockControl);
            this.Width = clockControl.Width;
            this.Dock = DockStyle.Right;
        }

        public override async Task SaveConfiguration()
        {
            await Task.CompletedTask;
        }
    }
}
