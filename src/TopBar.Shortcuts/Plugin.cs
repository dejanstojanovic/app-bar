using TopBar.Plugins.Shortcuts.Extensions;
using System.Text.Json;

namespace TopBar.Plugins.Shortcuts
{
    public class Plugin : PluginBase
    {

        readonly ContextMenuStrip _contextMenuStripMain;
        readonly ContextMenuStrip _contextMenuStripShortcut;
        readonly ColorTheme _colorTheme;
        readonly Configuration _configuration;

        public override event EventHandler ApplicationExit = null;
        public override event EventHandler ApplicationRestart = null;
        public bool ShowLabels { get; set; }

        public Plugin() : base()
        {
            _colorTheme = new ColorTheme();

            this.Dock = DockStyle.Fill;
            this.AllowDrop = true;
            this.DragDrop += Plugin_DragDrop;
            this.DragOver += Plugin_DragOver;

            #region Load configuration

            if (!string.IsNullOrWhiteSpace(this.ConfigurationFilePath))
            {
                string configurationContent = null;
                if (File.Exists(ConfigurationFilePath))
                    configurationContent = File.ReadAllText(ConfigurationFilePath);
                else
                    configurationContent = new StreamReader(this.GetType().Assembly.GetManifestResourceStream($"{this.GetType().Namespace}.{this.GetType().Namespace}.json")).ReadToEnd();

                _configuration = JsonSerializer.Deserialize<Configuration>(configurationContent);

                if (_configuration != null)
                    ShowLabels = _configuration.ShowLabels;
            }

            #endregion

            #region Main ContextMenu
            _contextMenuStripMain = new ContextMenuStrip()
            {
                RenderMode = ToolStripRenderMode.System,
            };

            _contextMenuStripMain.Items.Add(new ToolStripMenuItem("Labels", null,
                (sender, e) =>
                {
                    var contextItem = sender as ToolStripMenuItem;
                    this.ShowLabels = !this.ShowLabels;
                    var show = this.ShowLabels;
                    if (contextItem != null && show)
                        contextItem.Checked = true;
                    else if (contextItem != null)
                        contextItem.Checked = false;

                    foreach (Shortcut shortcut in this.Controls)
                    {
                        if (show)
                            shortcut.ShowLabel();
                        else
                            shortcut.HideLabel();

                    }
                    this.ReArrangeShortcuts();
                }, "ShowHide")
            {
                Checked = this.ShowLabels
            });

            _contextMenuStripMain.Items.Add("-");

            _contextMenuStripMain.Items.Add(new ToolStripMenuItem("Restart application", null,
                (sender, e) =>
                {
                    if (this.ApplicationRestart != null)
                        this.ApplicationRestart.Invoke(this, EventArgs.Empty);
                }, "Restart"));
            this.ContextMenuStrip = _contextMenuStripMain;

            _contextMenuStripMain.Items.Add("-");

            _contextMenuStripMain.Items.Add(new ToolStripMenuItem("Exit", null,
                (sender, e) =>
                {
                    if (this.ApplicationExit != null)
                        this.ApplicationExit.Invoke(this, EventArgs.Empty);
                }, "Exit"));
            this.ContextMenuStrip = _contextMenuStripMain;
            #endregion

            #region Shortcut ContextMenu

            _contextMenuStripShortcut = new ContextMenuStrip()
            {
                RenderMode = ToolStripRenderMode.System
            };
            _contextMenuStripShortcut.Items.Add(
            new ToolStripMenuItem("Open", null, (sender, e) =>
            {
                var item = sender as ToolStripMenuItem;
                if (item != null)
                {
                    var sourceControl = item.GetSourceControl() as Shortcut;
                    if (sourceControl != null)
                        sourceControl.OpenShortcut();
                }
            }, "Open"));
            _contextMenuStripShortcut.Items.Add(
            new ToolStripMenuItem("Remove", null, (sender, e) =>
            {
                var item = sender as ToolStripMenuItem;
                if (item != null)
                {
                    var sourceControl = item.GetSourceControl();
                    if (sourceControl != null)
                    {
                        sourceControl.Parent.Controls.Remove(sourceControl);
                        ReArrangeShortcuts();
                    }
                }

            }, "Remove"));

            _contextMenuStripShortcut.Closing += ContextMenuStripShortcut_Closing;
            #endregion

            #region Load saved shortcuts

            if (_configuration?.Shortcuts != null && _configuration.Shortcuts.Any())
                LoadShortcuts(_configuration.Shortcuts, _configuration.ShowLabels);

            #endregion
        }

        #region Event handle methods

        private void ContextMenuStripShortcut_Closing(object? sender, ToolStripDropDownClosingEventArgs e)
        {
            var contextMenu = sender as ContextMenuStrip;
            if (contextMenu == null)
                return;

            var sourceControl = contextMenu.SourceControl as Shortcut;
            if (sourceControl != null)
                sourceControl.UnHiglightAll();
        }

        private void Plugin_DragOver(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Plugin_DragDrop(object? sender, DragEventArgs e)
        {
            IEnumerable<string> files = e.Data != null && e.Data.GetData(DataFormats.FileDrop) != null ?
                                        e.Data.GetData(DataFormats.FileDrop) as IEnumerable<string> :
                                        null;

            if (files == null || !files.Any())
                return;

            LoadShortcuts(files.Select(f => f.IsDirectory() ? f : (Path.GetExtension(f).Equals(".lnk", StringComparison.InvariantCultureIgnoreCase) ? f.GetShortcutTarget() : f))
                .Select(f => new ShortcutConfiguration(f)),
                this.ShowLabels);
        }

        #endregion

        private void ReArrangeShortcuts()
        {
            this.SuspendLayout();
            var left = 4;
            foreach (Control control in this.Controls)
            {
                control.Left = left;
                left += control.Width + 4;
            }
            this.ResumeLayout();
        }

        private void LoadShortcuts(IEnumerable<ShortcutConfiguration> configs, bool showLabel)
        {
            foreach (var config in configs.Where(p =>
                                !string.IsNullOrWhiteSpace(p.Path))
                                .Where(p => File.Exists(Environment.ExpandEnvironmentVariables(p.Path)) || Directory.Exists(Environment.ExpandEnvironmentVariables(p.Path)))
                                .ToArray()
                                )
            {
                config.Path = Environment.ExpandEnvironmentVariables(config.Path);
                var shortcut = new Shortcut(config);

                if (showLabel)
                    shortcut.ShowLabel();
                else
                    shortcut.HideLabel();

                var left = 4;
                foreach (Control control in this.Controls)
                {
                    left += control.Width + 4;
                }
                shortcut.Left = left;
                shortcut.ContextMenuStrip = _contextMenuStripShortcut;
                this.Controls.Add(shortcut);
            }
        }

        public async override Task SaveConfig()
        {
            var shortcutControls = this.Controls.Cast<Shortcut>();
            var configuration = new Configuration()
            {
                ShowLabels = this.ShowLabels,
                Shortcuts = shortcutControls.Select(c => c.ShortcutConfiguration)
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