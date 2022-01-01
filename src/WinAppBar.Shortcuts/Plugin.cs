using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppBar.Shortcuts
{
    public class Plugin : Panel
    {
        public Plugin() : base()
        {
            this.Dock = DockStyle.Fill;
            this.AllowDrop = true;
            this.DragDrop += Plugin_DragDrop;
            this.DragOver += Plugin_DragOver;
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
            var fileExtensionsWithIcons = new String[] { ".exe", ".lnk" };

            IEnumerable<string> files = e.Data != null && e.Data.GetData(DataFormats.FileDrop) != null ?
                                        e.Data.GetData(DataFormats.FileDrop) as IEnumerable<string> :
                                        null;

            if (files == null || !files.Any())
                return;

            foreach (var file in files)
            {
                Icon icon = null;

                FileAttributes attr = File.GetAttributes(file);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
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
                        //Left = (this.Controls.Count * pictureBox.Width)+5
                    };
                    pictureBox.Left = 5+ (this.Controls.Count * pictureBox.Width) + (this.Controls.Count * 2);

                    pictureBox.MouseHover += PictureBox_MouseHover;
                    pictureBox.MouseLeave += PictureBox_MouseLeave;

                    this.Controls.Add(pictureBox);
                }
            }
        }

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
                    a = (Byte) ((color >> 24) & 0xFF),
                    b = (Byte)((color >> 16) & 0xFF),
                    g = (Byte)((color >> 8) & 0xFF),
                    r = (Byte)((color >> 0) & 0xFF);

                return (r, g, b, a);
            }

        }



        #endregion

        private void PictureBox_MouseLeave(object? sender, EventArgs e)
        {
            var control = sender as PictureBox;
            if (control != null)
                control.BackColor = Color.Transparent;
        }


        private void PictureBox_MouseHover(object? sender, EventArgs e)
        {
            var accentColor = GetAccentColor();
            var control = sender as PictureBox;
            if (control != null)
                control.BackColor = Color.FromArgb(accentColor.a,accentColor.r,accentColor.g,accentColor.b);
        }
    }
}