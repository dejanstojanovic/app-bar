namespace TopBar.Plugins.Shortcuts
{
    public partial class PluginOptionsForm : Form
    {
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
