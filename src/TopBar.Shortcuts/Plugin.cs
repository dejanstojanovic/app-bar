using TopBar.Plugins.Shortcuts.Extensions;
using System.Text.Json;
using TopBar.Plugins.Extensions;

namespace TopBar.Plugins.Shortcuts
{
    public class Plugin : PluginBase
    {
        readonly ContextMenuStrip _contextMenuStripShortcut;
        readonly ColorTheme _colorTheme;
        readonly Configuration _configuration;

        readonly IEnumerable<ToolStripMenuItem> _menuItems;

        public bool ShowLabels { get; set; }

        public override IEnumerable<ToolStripMenuItem> MenuItems => _menuItems;

        public override string Name => "Shortcuts";

        public Plugin(ColorTheme colorTheme) : base()
        {
            _colorTheme = colorTheme;

            this.Dock = DockStyle.Fill;
            this.AllowDrop = true;
            this.DragDrop += Plugin_DragDrop;
            this.DragOver += Plugin_DragOver;

            _configuration = LoadConfiguration(typeof(Configuration)) as Configuration ?? new Configuration();
            if (_configuration != null)
                ShowLabels = _configuration.ShowLabels;

            #region Plugin ContextMenu

            _menuItems = new ToolStripMenuItem[] {
                new ToolStripMenuItem("Labels", null,
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
                    },
                new ToolStripMenuItem("Add shortcut to file...", null,
                        (sender, e) =>
                        {
                            var dialog = new OpenFileDialog();
                            dialog.Multiselect = true;
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                this.AddShortcuts(dialog.FileNames.Select(f=> new ShortcutConfiguration(f)), this.ShowLabels);
                            }
                        },"AddFileShortcuts"),
                new ToolStripMenuItem("Add shortcut to folder...", null,
                        (sender, e) =>
                        {
                            var dialog = new FolderBrowserDialog();
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                this.AddShortcuts(new ShortcutConfiguration[] {new ShortcutConfiguration(dialog.SelectedPath)}, this.ShowLabels);
                            }
                        },"AddFolderShortcut"),
               new ToolStripMenuItem("Options...", null,
                        (sender, e) =>
                        {
                            ShowOptionsWinow();
                        },"Options"),
                };


            #endregion

            #region Shortcut ContextMenu

            _contextMenuStripShortcut = new ContextMenuStrip()
            {
                RenderMode = ToolStripRenderMode.System
            };
            _contextMenuStripShortcut.Items.AddRange(new ToolStripItem[] {
            new ToolStripMenuItem("Open", null, (sender, e) =>
            {
                var item = sender as ToolStripMenuItem;
                if (item != null)
                {
                    var sourceControl = item.GetSourceControl() as Shortcut;
                    if (sourceControl != null)
                        sourceControl.OpenShortcut();
                }
            }, "Open"),
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

            }, "Remove"),
            new ToolStripSeparator(),
            new ToolStripMenuItem("Options...", null,
                        (sender, e) =>
                        {
                            ShowOptionsWinow(this.Controls.IndexOf((sender as ToolStripMenuItem).GetSourceControl()));
                        },"ShortcutOptions")
            });

            _contextMenuStripShortcut.Closing += ContextMenuStripShortcut_Closing;
            #endregion

            #region Load saved shortcuts

            if (_configuration?.Shortcuts != null && _configuration.Shortcuts.Any())
                AddShortcuts(_configuration.Shortcuts, _configuration.ShowLabels);

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

            AddShortcuts(files.Select(f => f.IsDirectory() ? f : (Path.GetExtension(f).Equals(".lnk", StringComparison.InvariantCultureIgnoreCase) ? f.GetShortcutTarget() : f))
                .Select(f => new ShortcutConfiguration(f)),
                this.ShowLabels);
        }

        #endregion

        private void ShowOptionsWinow(int? index = null)
        {
            using (var optionsForm = index != null ?
                new PluginOptionsForm(this.Controls.Cast<Shortcut>(), index.Value) :
                new PluginOptionsForm(this.Controls.Cast<Shortcut>()))
            {
                if (optionsForm.ShowDialog() == DialogResult.OK)
                {
                    this.FindForm().LockWindowUpdate();
                    this.Controls.Clear();
                    this.AddShortcuts(optionsForm.ShortcutConfigurations, this.ShowLabels);
                    this.FindForm().UnlockWindowUpdate();
                }
            }
        }

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

        private void AddShortcuts(IEnumerable<ShortcutConfiguration> configs, bool showLabel)
        {
            foreach (var config in configs.Where(p =>
                                !string.IsNullOrWhiteSpace(p.Path))
                                .Where(p => File.Exists(Environment.ExpandEnvironmentVariables(p.Path)) || Directory.Exists(Environment.ExpandEnvironmentVariables(p.Path)))
                                .ToArray()
                                )
            {
                config.Path = Environment.ExpandEnvironmentVariables(config.Path);
                var shortcut = new Shortcut(config, _colorTheme);

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

        public async override Task SaveConfiguration()
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