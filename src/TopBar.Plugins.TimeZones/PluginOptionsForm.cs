using System.Data;
using TopBar.Plugins.Extensions;
using static System.Windows.Forms.ListViewItem;

namespace TopBar.Plugins.TimeZones
{
    public partial class PluginOptionsForm : Form
    {
        readonly ComboBox _timeZonesList;
        readonly ContextMenuStrip _contextMenuTimezones;
        private Configuration _configuration;

        public Configuration Configuration => _configuration;

        public PluginOptionsForm(Configuration configuration)
        {
            _configuration = configuration;
            _timeZonesList = new ComboBox() { Visible = false, DropDownStyle = ComboBoxStyle.DropDownList };

            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            _timeZonesList.DataSource = timeZones;
            _timeZonesList.ValueMember = nameof(TimeZoneInfo.Id);
            _timeZonesList.DisplayMember = nameof(TimeZoneInfo.DisplayName);

            this.Controls.Add(_timeZonesList);

            InitializeComponent();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            _contextMenuTimezones = new ContextMenuStrip()
            {
                RenderMode = ToolStripRenderMode.System
            };
            _contextMenuTimezones.VisibleChanged += _contextMenuTimezones_VisibleChanged;

            _contextMenuTimezones.Items.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Edit label", null, (sender, e) =>
                {
                    var item = sender as ToolStripMenuItem;
                    if (item != null)
                    {
                         var sourceControl = item.GetSourceControl() as ListView;
                        if (sourceControl != null)
                        {
                            var listItem =sourceControl.SelectedItems.Cast<ListViewItem>().SingleOrDefault();
                            if(listItem != null)
                                listItem.BeginEdit();
                        }
                    }
                }, "EditLabel"),
                new ToolStripMenuItem("Edit timezone", null, (sender, e) =>
                {
                    var item = sender as ToolStripMenuItem;
                    if (item != null)
                    {
                         var sourceControl = item.GetSourceControl() as ListView;
                        if (sourceControl != null)
                        {
                            var listItem =sourceControl.SelectedItems.Cast<ListViewItem>().SingleOrDefault();
                            if(listItem != null)
                                BeginEditTimezone(listItem);
                        }
                    }
                }, "EditTimezone"),
                new ToolStripMenuItem("Remove", null, (sender, e) =>
                {
                    var item = sender as ToolStripMenuItem;
                    if (item != null)
                    {
                         var sourceControl = item.GetSourceControl() as ListView;
                        if (sourceControl != null)
                        {
                            var listItem =sourceControl.SelectedItems.Cast<ListViewItem>().SingleOrDefault();
                            if(listItem != null)
                                sourceControl.Items.Remove(listItem);
                        }
                    }
                }, "Remove")
            });
            _listviewTimezones.ContextMenuStrip = _contextMenuTimezones;

            this._listviewTimezones.MouseDoubleClick += _listviewShorcuts_MouseDoubleClick;
            this._timeZonesList.LostFocus += _timeZonesList_LostFocus;
        }

        private void _contextMenuTimezones_VisibleChanged(object? sender, EventArgs e)
        {
            if (this._listviewTimezones.SelectedItems.Count == 0)
                this._contextMenuTimezones.Enabled = false;
            else
                this._contextMenuTimezones.Enabled = true;
        }

        private void _timeZonesList_LostFocus(object? sender, EventArgs e)
        {
            if (_timeZonesList.Visible)
            {
                //TODO: Do the thing
                var selectedItem = _listviewTimezones.SelectedItems.Cast<ListViewItem>().SingleOrDefault();
                if (selectedItem != null)
                {
                    selectedItem.SubItems[1].Text = (_timeZonesList.SelectedItem as TimeZoneInfo)?.DisplayName;
                    selectedItem.Tag = _timeZonesList.SelectedItem as TimeZoneInfo;

                }

                _timeZonesList.Hide();
            }
        }

        private void _listviewShorcuts_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            var currentItem = _listviewTimezones.GetItemAt(e.X, e.Y);
            if (currentItem == null)
                return;

            var currentSubItem = currentItem.GetSubItemAt(e.X, e.Y);
            if (currentSubItem == null)
                return;

            var lLeft = currentSubItem.Bounds.Left + 2;
            var lWidth = currentSubItem.Bounds.Width;


            if (currentSubItem.Name.Equals(nameof(ClockConfiguration.TimeZoneId)))
            {
                _timeZonesList.SelectedItem = _timeZonesList.Items.Cast<TimeZoneInfo>().SingleOrDefault(z => z.Id == (currentItem.Tag as TimeZoneInfo)?.Id);
                _timeZonesList.SetBounds(lLeft + _listviewTimezones.Left, currentSubItem.Bounds.Top + _listviewTimezones.Top, lWidth, currentSubItem.Bounds.Height);
                _timeZonesList.Show();
                _timeZonesList.Focus();
            }
        }

        private void BeginEditTimezone(ListViewItem item)
        {
            var subItem = item.SubItems[1];
            var lLeft = subItem.Bounds.Left + 2;
            var lWidth = subItem.Bounds.Width;

            _timeZonesList.SelectedItem = _timeZonesList.Items.Cast<TimeZoneInfo>().SingleOrDefault(z => z.Id == (item.Tag as TimeZoneInfo)?.Id);
            _timeZonesList.SetBounds(lLeft + _listviewTimezones.Left, subItem.Bounds.Top + _listviewTimezones.Top, lWidth, subItem.Bounds.Height);
            _timeZonesList.Show();
            _timeZonesList.Focus();
        }

        private void PluginOptionsForm_Load(object sender, EventArgs e)
        {
            foreach (var time in _configuration.DatesAndTimes)
            {
                var item = new ListViewItem()
                {
                    Name = nameof(ClockConfiguration.Title),
                    Text = time.Title,
                    Tag = time.TimeZone,
                };
                item.SubItems.Add(new ListViewSubItem(item, time.TimeZone.DisplayName)
                {
                    Name = nameof(ClockConfiguration.TimeZoneId),
                });
                this._listviewTimezones.Items.Add(item);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this._configuration = new Configuration()
            {
                DatesAndTimes = this._listviewTimezones.Items.Cast<ListViewItem>()
                                                .Select(x => new ClockConfiguration()
                                                {
                                                    TimeZoneId = (x.Tag as TimeZoneInfo)?.Id,
                                                    Title = x.Text
                                                })
            };
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var usedTimeZoneIds = this._listviewTimezones.Items.Cast<ListViewItem>()
                                                .Select(x => x.Tag as TimeZoneInfo)
                                                .Select(z => z?.Id)
                                                .Where(z => !string.IsNullOrWhiteSpace(z))
                                                .ToArray();

            var unusedTimeZones = this._timeZonesList.Items.Cast<TimeZoneInfo>()
                .Where(t => !usedTimeZoneIds.Contains(t.Id));

            if (unusedTimeZones == null || !unusedTimeZones.Any())
                return;

            var timeZone = unusedTimeZones.First();

            var item = new ListViewItem($"UTC{(timeZone.BaseUtcOffset.TotalHours >= 0 ? "+" : String.Empty)}{timeZone.BaseUtcOffset.Hours.ToString().PadLeft(2, '0')}:{timeZone.BaseUtcOffset.Minutes.ToString().PadLeft(2, '0')}")
            {
                Tag = timeZone
            };
            item.SubItems.Add(new ListViewSubItem(item, timeZone.DisplayName)
            {
                Name = nameof(ClockConfiguration.TimeZoneId),
            });

            this._listviewTimezones.Items.Add(item);
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            var selected = _listviewTimezones.SelectedItems != null ? _listviewTimezones.SelectedItems.Cast<ListViewItem>().SingleOrDefault() : null;
            if (selected != null && selected.Index > 0)
            {
                var index = selected.Index;
                _listviewTimezones.Items.RemoveAt(index);
                _listviewTimezones.Items.Insert(index - 1, selected);
            }
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            var selected = _listviewTimezones.SelectedItems != null ? _listviewTimezones.SelectedItems.Cast<ListViewItem>().SingleOrDefault() : null;
            if (selected != null && selected.Index < _listviewTimezones.Items.Count - 1)
            {
                var index = selected.Index;
                _listviewTimezones.Items.RemoveAt(index);
                _listviewTimezones.Items.Insert(index + 1, selected);
            }
        }
    }
}
