namespace TopBar.Plugins.Shortcuts
{
    internal partial class PluginOptionsForm : Form
    {
        IEnumerable<Shortcut> _shortcuts;
        readonly ImageList _imageList;

        public IEnumerable<ShortcutConfiguration> ShortcutConfigurations => _shortcuts?.Select(s => s.ShortcutConfiguration);

        public PluginOptionsForm(IEnumerable<Shortcut> shortcuts) : this()
        {
            buttonMoveDown.Enabled = false;
            buttonMoveUp.Enabled = false;

            _shortcuts = shortcuts;

            _imageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
            };

            _imageList.Images.AddRange(shortcuts.Select(s => s.Icon).ToArray());

            var shortcutsList = _shortcuts.ToList();

            this._listviewShorcuts.SmallImageList = _imageList;
            this._listviewShorcuts.Items.AddRange(
                _shortcuts.Select(s => new ListViewItem()
                {
                    Text = s.ShortcutConfiguration.Label,
                    ImageIndex = shortcutsList.IndexOf(s),
                    Tag = s
                }).ToArray());
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
            this._shortcuts = _listviewShorcuts.Items.Cast<ListViewItem>()
                                    .OrderBy(i => i.Index)
                                    .Select(i => i.Tag as Shortcut)
                                    .ToArray();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        private void PluginOptions_Load(object sender, EventArgs e)
        {

        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            var selected = _listviewShorcuts.SelectedItems != null ? _listviewShorcuts.SelectedItems.Cast<ListViewItem>().SingleOrDefault() : null;
            if (selected != null && selected.Index > 0)
            {
                var index = selected.Index;
                _listviewShorcuts.Items.RemoveAt(index);
                _listviewShorcuts.Items.Insert(index - 1, selected);
            }


        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            var selected = _listviewShorcuts.SelectedItems != null ? _listviewShorcuts.SelectedItems.Cast<ListViewItem>().SingleOrDefault() : null;
            if (selected != null && selected.Index < _listviewShorcuts.Items.Count - 1)
            {
                var index = selected.Index;
                _listviewShorcuts.Items.RemoveAt(index);
                _listviewShorcuts.Items.Insert(index + 1, selected);
            }
        }

        private void _listviewShorcuts_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = _listviewShorcuts.SelectedItems != null ? _listviewShorcuts.SelectedItems.Cast<ListViewItem>().SingleOrDefault() : null;
            if (selected != null)
            {
                if (selected.Index == _listviewShorcuts.Items.Count - 1)
                    buttonMoveDown.Enabled = false;
                else
                    buttonMoveDown.Enabled = true;

                if (selected.Index == 0)
                    buttonMoveUp.Enabled = false;
                else
                    buttonMoveUp.Enabled = true;
            }
            else
            {
                buttonMoveDown.Enabled = false;
                buttonMoveUp.Enabled = false;
            }
        }
    }
}
