namespace TopBar.Plugins
{
    public interface IPlugin
    {
        string Name { get; }
        int Order { get; }
        IEnumerable<ToolStripMenuItem> MenuItems { get; }
        Task SaveConfiguration();
    }
}
