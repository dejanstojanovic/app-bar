﻿namespace WinAppBar.Plugins
{
    public interface IPlugin
    {
        event EventHandler ApplicationExit;
        Task SaveConfig();
        Task<String> LoadConfig();
    }
}
