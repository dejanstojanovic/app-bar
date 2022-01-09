namespace TopBar.Plugins
{
    public interface IPlugin
    {
        string Name { get; }
        IEnumerable<ToolStripMenuItem> MenuItems { get; }
        Task SaveConfiguration();
    }
}
