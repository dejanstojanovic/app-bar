using System.Text.Json;
using TopBar.Plugins.Extensions;

namespace TopBar.Plugins.WorldClock
{
    public class Plugin : PluginBase
    {
        readonly ColorTheme _colorTheme;
        readonly IEnumerable<ToolStripMenuItem> _menuItems;


        public override IEnumerable<ToolStripMenuItem> MenuItems => _menuItems;
        public override string Name => "World Clock";
        public override int Order => 1;
        public Plugin(ColorTheme colorTheme) : base()
        {
            _colorTheme = colorTheme;
            this.Dock = DockStyle.Right;


            var configuration = this.LoadConfiguration(typeof(Configuration)) as Configuration;
            if(configuration != null)
                AddTimeZones(configuration);

            _menuItems = new ToolStripMenuItem[] {
                new ToolStripMenuItem("Settings...", null,
                (sender, e) =>
                    {
                         ShowOptionsWinow();
                    },"Settings")
                };

        }

        private void AddTimeZones(Configuration configuration)
        {
            var form = this.FindForm();
            if(form != null)
                form.LockWindowUpdate();

            this.Controls.Clear();
            foreach (var item in configuration.DatesAndTimes)
            {
                var dateTimeControl = new DateTimeControl(_colorTheme, new ClockConfiguration()
                {
                    TimeZoneId = item.TimeZoneId,
                    Title = item.Title
                })
                { Dock = DockStyle.Right };
                this.Controls.Add(dateTimeControl);
            }
            this.Width = this.Controls.Cast<Control>().Sum(c => c.Width);

            if (form != null)
                form.UnlockWindowUpdate();
        }

        private void ShowOptionsWinow()
        {
            using (var optionsForm = new PluginOptionsForm(new Configuration()
            {
                DatesAndTimes = this.Controls.Cast<DateTimeControl>().Select(c => new ClockConfiguration()
                {
                    Title = c.Configuration.Title,
                    TimeZoneId = c.Configuration.TimeZoneId
                }).ToArray()
            }))
            {
                if (optionsForm.ShowDialog() == DialogResult.OK)
                {
                    this.AddTimeZones(optionsForm.Configuration);
                }
            }
        }

        public override async Task SaveConfiguration()
        {
            var timeControls = this.Controls.Cast<DateTimeControl>();
            var configuration = new Configuration()
            {
                DatesAndTimes = timeControls.Select(c => c.Configuration)
            };
            var configContent = JsonSerializer.Serialize<Configuration>(
                 configuration,
                 new JsonSerializerOptions()
                 {
                     WriteIndented = true
                 });

            await File.WriteAllTextAsync(path: this.ConfigurationFilePath, configContent);
        }
    }
}
