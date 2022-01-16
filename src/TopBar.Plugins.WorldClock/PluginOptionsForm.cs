using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace TopBar.Plugins.WorldClock
{
    public partial class PluginOptionsForm : Form
    {
        readonly ComboBox _timeZonesList;
        readonly Configuration _configuration;

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

            this._listviewShorcuts.MouseDoubleClick += _listviewShorcuts_MouseDoubleClick;
            this._timeZonesList.LostFocus += _timeZonesList_LostFocus;
        }

        private void _timeZonesList_LostFocus(object? sender, EventArgs e)
        {
            if (_timeZonesList.Visible)
            {
                //TODO: Do the thing
                var selectedItem = _listviewShorcuts.SelectedItems.Cast<ListViewItem>().SingleOrDefault();
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


            var currentItem = _listviewShorcuts.GetItemAt(e.X, e.Y);
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
                _timeZonesList.SetBounds(lLeft + _listviewShorcuts.Left, currentSubItem.Bounds.Top + _listviewShorcuts.Top, lWidth, currentSubItem.Bounds.Height);
                _timeZonesList.Show();
                _timeZonesList.Focus();
            }
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
                this._listviewShorcuts.Items.Add(item);
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
