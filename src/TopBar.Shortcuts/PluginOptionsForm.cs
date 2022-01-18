using TopBar.Plugins.Shortcuts.Extensions;

namespace TopBar.Plugins.Shortcuts
{
    internal partial class PluginOptionsForm : Form
    {
        IEnumerable<Shortcut> _shortcuts;
        readonly ImageList _imageList;
        readonly ContextMenuStrip _contextMenuStripShortcut;

        public IEnumerable<ShortcutConfiguration> ShortcutConfigurations => _shortcuts?.Select(s => s.ShortcutConfiguration);

        public PluginOptionsForm(IEnumerable<Shortcut> shortcuts, int index) : this(shortcuts)
        {
            this._listviewShorcuts.Items[index].Selected = true;
            this._listviewShorcuts.Select();
        }

        public PluginOptionsForm(IEnumerable<Shortcut> shortcuts) : this()
        {
            buttonMoveDown.Enabled = false;
            buttonMoveUp.Enabled = false;

            _shortcuts = shortcuts;

            _contextMenuStripShortcut = new ContextMenuStrip()
            {
                RenderMode = ToolStripRenderMode.System
            };
            _contextMenuStripShortcut.VisibleChanged += _contextMenuStripShortcut_VisibleChanged;

            _contextMenuStripShortcut.Items.AddRange(new ToolStripItem[] {
            new ToolStripMenuItem("Edit label", null, (sender, e) =>
            {
                var item = sender as ToolStripMenuItem;
                if (item != null)
                {
                     var sourceControl = item.GetContextControl() as ListView;
                    if (sourceControl != null)
                    {
                        var listItem =sourceControl.SelectedItems.Cast<ListViewItem>().SingleOrDefault();
                        if(listItem != null)
                            listItem.BeginEdit();
                    }
                }
            }, "Edit"),
            new ToolStripMenuItem("Remove shortcut", null, (sender, e) =>
            {
                var item = sender as ToolStripMenuItem;
                if (item != null)
                {
                    var sourceControl = item.GetContextControl() as ListView;
                    if (sourceControl != null)
                    {
                        var listItem = sourceControl.SelectedItems.Cast<ListViewItem>().SingleOrDefault();
                        if (listItem != null)
                            sourceControl.Items.Remove(listItem);
                    }
                }
            }, "Remove") });
            _listviewShorcuts.ContextMenuStrip = _contextMenuStripShortcut;

            _imageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
            };

            _imageList.Images.AddRange(shortcuts.Select(s => s.Icon).ToArray());

            var shortcutsList = _shortcuts.ToList();

            this._listviewShorcuts.AfterLabelEdit += _listviewShorcuts_AfterLabelEdit;

            this._listviewShorcuts.SmallImageList = _imageList;

            this._listviewShorcuts.Items.AddRange(_shortcuts.Select(s =>
            {
                var item = new ListViewItem()
                {
                    Text = s.ShortcutConfiguration.Label,
                    ImageIndex = shortcutsList.IndexOf(s),
                    Tag = s
                };
                item.SubItems.Add(s.ShortcutConfiguration.Path);
                return item;
            }).ToArray());

        }

        private void _contextMenuStripShortcut_VisibleChanged(object? sender, EventArgs e)
        {
            if(this._listviewShorcuts.SelectedItems.Count == 0)
                this._contextMenuStripShortcut.Enabled = false;
            else
                this._contextMenuStripShortcut.Enabled = true;
        }

        private void _listviewShorcuts_AfterLabelEdit(object? sender, LabelEditEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Label))
                e.CancelEdit = true;
            else
            {
                var item = _listviewShorcuts.Items[e.Item];
                if (item != null)
                {
                    var shortcut = item.Tag as Shortcut;
                    if (shortcut != null && shortcut.ShortcutConfiguration != null)
                        shortcut.ShortcutConfiguration.Label = e.Label;
                }
            }
        }

        public PluginOptionsForm()
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
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
