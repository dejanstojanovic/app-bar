using System.Reflection;
using WinAppBar.Plugins.Shortcuts.Extensions;
using System.Text.Json;

namespace WinAppBar.Plugins.Shortcuts
{
    public class Plugin : PluginBase
    {

        readonly ContextMenuStrip contextMenuStripMain;
        readonly ContextMenuStrip contextMenuStripShortcut;

        public override event EventHandler ApplicationExit = null;
        public bool ShowLabels { get; set; }

        public Plugin() : base()
        {
            this.Dock = DockStyle.Fill;
            this.AllowDrop = true;
            this.DragDrop += Plugin_DragDrop;
            this.DragOver += Plugin_DragOver;

            #region Main ContextMenu
            contextMenuStripMain = new ContextMenuStrip()
            {
                RenderMode = ToolStripRenderMode.System,
            };

            contextMenuStripMain.Items.Add(new ToolStripMenuItem("Show/hide labels", null,
                (sender, e) =>
                {
                    this.ShowLabels = !this.ShowLabels;
                    foreach (Shortcut shortcut in this.Controls)
                    {
                        //shortcut.ToggleLabel();
                        if (this.ShowLabels)
                            shortcut.ShowLabel();
                        else
                            shortcut.HideLabel();
                    }
                    this.ReArrangeShortcuts();
                }, "ShowHide"));

            contextMenuStripMain.Items.Add("-");

            contextMenuStripMain.Items.Add(new ToolStripMenuItem("Exit", null,
                (sender, e) =>
                {
                    if (this.ApplicationExit != null)
                        this.ApplicationExit.Invoke(this, EventArgs.Empty);
                }, "Exit"));
            this.ContextMenuStrip = contextMenuStripMain;
            #endregion

            #region Shortcut ContextMenu

            contextMenuStripShortcut = new ContextMenuStrip()
            {
                RenderMode = ToolStripRenderMode.System
            };
            contextMenuStripShortcut.Items.Add(
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
            contextMenuStripShortcut.Items.Add(
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

            contextMenuStripShortcut.Closing += ContextMenuStripShortcut_Closing;
            #endregion


            #region Load configuration

            if (!string.IsNullOrWhiteSpace(this.ConfigurationFilePath) && File.Exists(this.ConfigurationFilePath))
            {
                string configurationContent = null;
                if (File.Exists(ConfigurationFilePath))
                    configurationContent = File.ReadAllText(ConfigurationFilePath);

                var configuration = JsonSerializer.Deserialize<Configuration>(configurationContent);

                if (configuration != null)
                    ShowLabels = configuration.ShowLabels;

                if (configuration?.Shortcuts != null && configuration.Shortcuts.Any())
                    LoadShortcuts(configuration.Shortcuts.Select(s => s.Path).ToArray(), configuration.ShowLabels);
            }

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
                sourceControl.UnHiglight();
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

            LoadShortcuts(files, this.ShowLabels);
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

        private void LoadShortcuts(IEnumerable<string> paths, bool showLabel)
        {
            var fileExtensionsWithIcons = new String[] { ".exe", ".lnk", ".ico" };

            foreach (var path in paths.Where(p =>
                                !string.IsNullOrWhiteSpace(p) &&
                                (File.Exists(p) || Directory.Exists(p))
            ).ToArray())
            {
                Icon icon = null;
                if (path.IsDirectory())
                    icon = IconTools.GetIconForFile(path, ShellIconSize.SmallIcon);
                else
                {
                    var extension = Path.GetExtension(path);
                    icon = fileExtensionsWithIcons.Any(e => e.Equals(extension)) ?
                        IconTools.GetIconForFile(path, ShellIconSize.SmallIcon) :
                        IconTools.GetIconForExtension(extension, ShellIconSize.SmallIcon);
                }

                if (icon != null)
                {
                    var shortcut = new Shortcut(path)
                    {
                        Icon = icon.ToBitmap(),
                        Label = Path.GetFileName(path),
                        Top = 4,
                        Tag = path
                    };

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
                    shortcut.ContextMenuStrip = contextMenuStripShortcut;
                    this.Controls.Add(shortcut);


                }
            }
        }

        public async override Task SaveConfig()
        {
            var shortcutControls = this.Controls.Cast<Shortcut>();
            var configuration = new Configuration()
            {
                ShowLabels = this.ShowLabels,
                Shortcuts = shortcutControls.Select(c => new ShortcutConfiguration()
                {
                    Path = c.Tag.ToString()
                })
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