﻿using Microsoft.Win32;
using System.Text.Json.Serialization;

namespace TopBar
{
    public class Configuration
    {
        const string StartupRunRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public string ScreenDeviceName { get; set; }

        [JsonIgnore]
        public bool RunOnStartup
        {
            get
            {
                var startupRunKey = Registry.CurrentUser.OpenSubKey(StartupRunRegistryKey, false);
                if (startupRunKey != null)
                {
                    var value = startupRunKey.GetValue(this.GetType().Namespace);
                    return value != null;
                }
                return false;
            }
            set
            {
                var startupRunKey = Registry.CurrentUser.OpenSubKey(StartupRunRegistryKey, true);
                if (value)
                    startupRunKey.SetValue(this.GetType().Namespace, $"\"{Application.ExecutablePath}\"");
                else
                    startupRunKey.DeleteValue(this.GetType().Namespace);
            }
        }
    }
}
