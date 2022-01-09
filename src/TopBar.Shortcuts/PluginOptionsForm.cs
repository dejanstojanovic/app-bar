namespace TopBar.Plugins.Shortcuts
{
    internal partial class PluginOptionsForm : Form
    {
        readonly IEnumerable<Shortcut> _shortcuts;

        public IEnumerable<Shortcut> Shortcuts => _shortcuts;

        public PluginOptionsForm(IEnumerable<Shortcut> shortcuts):this()
        {
            _shortcuts = shortcuts;
        }

        public PluginOptionsForm()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        private void PluginOptions_Load(object sender, EventArgs e)
        {

        }
    }
}
