namespace TopBar.Plugins
{
    public interface IPlugin
    {
        //event EventHandler ApplicationExit;
        //event EventHandler ApplicationRestart;

        string Name { get; }
        IEnumerable<ToolStripMenuItem> MenuItems { get; }
        Task SaveConfig();
    }
}
