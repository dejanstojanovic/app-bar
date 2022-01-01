using Microsoft.Win32;
using System.Diagnostics;
using WinAppBar.Plugins;

namespace WinAppBar.Shortcuts
{
    public class Plugin : Panel, IPlugin
    {

        #region Win API

        public static (Byte r, Byte g, Byte b, Byte a) GetAccentColor()
        {
            const String DWM_KEY = @"Software\Microsoft\Windows\DWM";
            using (RegistryKey dwmKey = Registry.CurrentUser.OpenSubKey(DWM_KEY, RegistryKeyPermissionCheck.ReadSubTree))
            {
                const String KEY_EX_MSG = "The \"HKCU\\" + DWM_KEY + "\" registry key does not exist.";
                if (dwmKey is null) throw new InvalidOperationException(KEY_EX_MSG);

                Object accentColorObj = dwmKey.GetValue("AccentColor");
                if (accentColorObj is Int32 accentColorDword)
                {
                    return ParseDWordColor(accentColorDword);
                }
                else
                {
                    const String VALUE_EX_MSG = "The \"HKCU\\" + DWM_KEY + "\\AccentColor\" registry key value could not be parsed as an ABGR color.";
                    throw new InvalidOperationException(VALUE_EX_MSG);
                }
            }

            static (Byte r, Byte g, Byte b, Byte a) ParseDWordColor(Int32 color)
            {
                Byte
                    a = (Byte)((color >> 24) & 0xFF),
                    b = (Byte)((color >> 16) & 0xFF),
                    g = (Byte)((color >> 8) & 0xFF),
                    r = (Byte)((color >> 0) & 0xFF);

                return (r, g, b, a);
            }
        }

        #endregion Win API

        readonly ToolTip toolTip;
        readonly ContextMenuStrip contextMenuStrip;

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

            contextMenuStrip = new ContextMenuStrip()
            {
                RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            };
            ToolStripMenuItem exitToolStripMenuItem = new ToolStripMenuItem("Exit", null, null, "Exit");
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;

            contextMenuStrip.Items.Add(exitToolStripMenuItem);
            this.ContextMenuStrip = contextMenuStrip;
        }

        private void ExitToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (this.ApplicationExit != null)
                this.ApplicationExit.Invoke(this, EventArgs.Empty);
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

        private bool IsDirectory(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return true;

            return false;

        }

        private void LoadShortcuts(IEnumerable<string> files)
        {
            var fileExtensionsWithIcons = new String[] { ".exe", ".lnk" };

            foreach (var file in files)
            {
                Icon icon = null;
                if (IsDirectory(file))
                    icon = IconTools.GetIconForFile(file, ShellIconSize.SmallIcon);
                else
                {
                    var extension = Path.GetExtension(file);
                    icon = fileExtensionsWithIcons.Any(e => e.Equals(extension)) ?
                        IconTools.GetIconForFile(file, ShellIconSize.SmallIcon) :
                        IconTools.GetIconForExtension(extension, ShellIconSize.SmallIcon);
                }

                if (icon != null)
                {
                    var pictureBox = new PictureBox()
                    {
                        Image = icon.ToBitmap(),
                        Size = new Size(24, 24),
                        Padding = new Padding(4),
                        Top = 5,
                        Tag = file
                    };
                    pictureBox.Left = 5 + (this.Controls.Count * pictureBox.Width) + (this.Controls.Count * 2);

                    pictureBox.MouseEnter += PictureBox_MouseEnter;
                    pictureBox.MouseLeave += PictureBox_MouseLeave;
                    pictureBox.MouseHover += PictureBox_MouseHover;
                    pictureBox.Click += PictureBox_Click;

                    this.Controls.Add(pictureBox);
                }
            }
        }

        private void PictureBox_MouseHover(object? sender, EventArgs e)
        {
            var btn = sender as Control;
            if (btn != null)
            {
                toolTip.SetToolTip(btn, btn.Tag.ToString());
            }
        }

        private void PictureBox_Click(object? sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            if (args != null && args.Button == MouseButtons.Right)
                return;

            var control = sender as PictureBox;
            if (control == null)
                return;

            var path = control.Tag.ToString();
            if (string.IsNullOrEmpty(path))
                return;

            var processInfo = new ProcessStartInfo();
            var executables = new String[] { ".exe", ".com", ".bat" };
            if (IsDirectory(path) ||
                !executables.Any(e => e.Equals(Path.GetExtension(path), StringComparison.InvariantCultureIgnoreCase)))
            {
                processInfo.FileName = path;
                processInfo.UseShellExecute = true;
            }
            else
            {
                processInfo.FileName = path;
                processInfo.UseShellExecute = false;
            }

            Process.Start(processInfo);
        }

        private void PictureBox_MouseLeave(object? sender, EventArgs e)
        {
            var control = sender as PictureBox;
            if (control != null)
                control.BackColor = Color.Transparent;
        }

        private void PictureBox_MouseEnter(object? sender, EventArgs e)
        {
            var accentColor = GetAccentColor();
            var control = sender as PictureBox;
            if (control != null)
                control.BackColor = Color.FromArgb(accentColor.a, accentColor.r, accentColor.g, accentColor.b);
        }
    }
}