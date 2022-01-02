using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppBar.Plugins
{
    public static class ColorUtils
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
    }
}
