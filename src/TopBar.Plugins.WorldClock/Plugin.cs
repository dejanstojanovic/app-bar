﻿using System;
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

        readonly DateTimeControl _dateTimeControl;

        public override IEnumerable<ToolStripMenuItem> MenuItems => _menuItems;
        public override string Name => "World Clock";

        public Plugin(ColorTheme colorTheme) : base()
        {
            _colorTheme = colorTheme;

            _dateTimeControl = new DateTimeControl(_colorTheme, new ClockConfiguration()
            {
                Active = true,
                TimeZoneId = "Dubai GMT+04",
                Title = "UAE"
            })
            { Dock = DockStyle.Right };


            this.Dock = DockStyle.Right;

            this.Controls.Add(_dateTimeControl);
            this.Width = _dateTimeControl.Width;

            _menuItems = new ToolStripMenuItem[] {
                new ToolStripMenuItem("Settings...", null,
                (sender, e) =>
                    {
                         var dialog = new OpenFileDialog();
                            dialog.Multiselect = true;
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                
                            }
                    },"Settings")
                };



        }

        public override async Task SaveConfiguration()
        {
            await Task.CompletedTask;
        }
    }
}
