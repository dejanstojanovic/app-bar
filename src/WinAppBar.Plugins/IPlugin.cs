namespace WinAppBar.Plugins
{
    public interface IPlugin
    {
        event EventHandler ApplicationExit;
        event EventHandler ApplicationRestart;

        Task SaveConfig();
    }
}
