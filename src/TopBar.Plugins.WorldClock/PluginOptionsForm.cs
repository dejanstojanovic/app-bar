using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopBar.Plugins.Extensions;
using static System.Windows.Forms.ListViewItem;

namespace TopBar.Plugins.WorldClock
{
    public partial class PluginOptionsForm : Form
    {
        readonly ComboBox _timeZonesList;
        readonly Configuration _configuration;
        readonly ContextMenuStrip _contextMenuTimezones;
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


            _contextMenuTimezones = new ContextMenuStrip()
            {
                RenderMode = ToolStripRenderMode.System
            };
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
                }, "EditTimezone")
            });
            _listviewTimezones.ContextMenuStrip = _contextMenuTimezones;

            this._listviewTimezones.MouseDoubleClick += _listviewShorcuts_MouseDoubleClick;
            this._timeZonesList.LostFocus += _timeZonesList_LostFocus;
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
            //https://www.codeproject.com/Articles/316204/ListView-in-line-editing


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

            _timeZonesList.SetBounds(lLeft + _listviewTimezones.Left, subItem.Bounds.Top + _listviewTimezones.Top, lWidth, subItem.Bounds.Height);
            _timeZonesList.Show();
            _timeZonesList.Focus();
        }

        private void PluginOptionsForm_Load(object sender, EventArgs e)
        {
            foreach (var time in _configuration.DatesAndTimes)
            {
                var item = new ListViewItem() { 
                    Name = nameof(ClockConfiguration.Title), 
                    Text = time.Title,
                    Tag = time.TimeZone,
                };
                item.SubItems.Add(new ListViewSubItem(item, time.TimeZone.DisplayName) { 
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
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void _listviewShorcuts_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
