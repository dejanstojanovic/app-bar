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
        public override string Name => "World Clock";
        public override IEnumerable<ToolStripMenuItem> MenuItems => null;

        public Plugin() : base()
        {
            this.BackColor = Color.Green;


            _colorTheme = new ColorTheme();
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
