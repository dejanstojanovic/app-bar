using Microsoft.Win32;
using System.Diagnostics;
using WinAppBar.Plugins;
using WinAppBar.Plugins.Shortcuts.Extensions;

namespace WinAppBar.Plugins.Shortcuts
{
    public class Plugin : Panel, IPlugin
    {



        readonly ToolTip toolTip;
        readonly ContextMenuStrip contextMenuStripMain;
        readonly ContextMenuStrip contextMenuStripShortcut;

        public event EventHandler ApplicationExit = null;

        public Plugin() : base()
        {
            this.Dock = DockStyle.Fill;
            this.AllowDrop = true;
            this.DragDrop += Plugin_DragDrop;
            this.DragOver += Plugin_DragOver;

            toolTip = new ToolTip()
            {
                AutoPopDelay = 0,
                InitialDelay = 1,
                ReshowDelay = 0,
                ShowAlways = true
            };


            #region Main ContextMenu
            contextMenuStripMain = new ContextMenuStrip()
            {
                RenderMode = ToolStripRenderMode.System,
            };
            ToolStripMenuItem exitToolStripMenuItem = new ToolStripMenuItem("Exit", null,
                (sender, e) =>
                {
                    if (this.ApplicationExit != null)
                        this.ApplicationExit.Invoke(this, EventArgs.Empty);
                }, "Exit");
            contextMenuStripMain.Items.Add(exitToolStripMenuItem);
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
        }

        private void ContextMenuStripShortcut_Closing(object? sender, ToolStripDropDownClosingEventArgs e)
        {
            var contextMenu = sender as ContextMenuStrip;
            if (contextMenu == null)
                return;

            var sourceControl = contextMenu.SourceControl as Shortcut;
            if (sourceControl != null)
                sourceControl.Unfocus();
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

            LoadShortcuts(files);
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

        private void LoadShortcuts(IEnumerable<string> paths)
        {
            var fileExtensionsWithIcons = new String[] { ".exe", ".lnk", ".ico" };

            foreach (var path in paths)
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

                    var left = 4;
                    foreach(Control control  in this.Controls)
                    {
                        left += control.Width + 4;
                    }
                    shortcut.Left = left;

                    shortcut.MouseHover += Shortcut_MouseHover;
                    
                    shortcut.ContextMenuStrip = contextMenuStripShortcut;

                    this.Controls.Add(shortcut);

                   
                }
            }
        }

        private void Shortcut_MouseHover(object? sender, EventArgs e)
        {
            var btn = sender as Control;
            if (btn != null)
            {
                toolTip.SetToolTip(btn, btn.Tag.ToString());
            }
        }


    }
}